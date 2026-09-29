using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using ClinicaMedEduardoMorenoMVCWeb.Models.ViewModels;
using ClinicaMedEduardoMorenoMVCWeb.Data;

namespace ClinicaMedEduardoMorenoMVCWeb.Controllers
{
    public class ExpedientesController : Controller
    {
        private readonly AppDbContext _context;

        public ExpedientesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Expedientes
        public async Task<IActionResult> Index(string? buscar, string? sexo, DateTime? creadoDesde, DateTime? creadoHasta)
        {
            var query = _context.Expedientes
                .Include(e => e.Paciente)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
                query = query.Where(e =>
                    e.Codigo.Contains(buscar) ||
                    (e.Paciente != null && e.Paciente.Nombre.Contains(buscar)));

            if (!string.IsNullOrWhiteSpace(sexo) && sexo != "Todos")
                query = query.Where(e => e.Paciente != null && e.Paciente.Sexo == sexo);

            if (creadoDesde.HasValue)
                query = query.Where(e => e.FechaCreacion >= creadoDesde.Value);

            if (creadoHasta.HasValue)
                query = query.Where(e => e.FechaCreacion <= creadoHasta.Value.AddDays(1));

            ViewBag.Buscar = buscar;
            ViewBag.Sexo = sexo ?? "Todos";
            ViewBag.CreadoDesde = creadoDesde?.ToString("yyyy-MM-dd");
            ViewBag.CreadoHasta = creadoHasta?.ToString("yyyy-MM-dd");

            var lista = await query.OrderByDescending(e => e.FechaCreacion).ToListAsync();
            return View(lista);
        }

        // GET: Expedientes/Details/5
        public async Task<IActionResult> Details(int? id, string tab = "alergias")
        {
            if (id == null) return NotFound();

            var expediente = await _context.Expedientes
                .Include(e => e.Paciente)
                .FirstOrDefaultAsync(m => m.ExpedienteId == id);

            if (expediente == null) return NotFound();

            var alergias = await _context.ExpedienteAlergias
                .Include(e => e.Alergia)
                .Where(e => e.ExpedienteId == id)
                .ToListAsync();

            var enfermedades = await _context.ExpedienteEnfermedades
                .Include(e => e.Enfermedad)
                .Where(e => e.ExpedienteId == id)
                .ToListAsync();

            var antecedentes = await _context.Antecedentes
                .Where(a => a.ExpedienteId == id)
                .ToListAsync();

            var consultas = await _context.Consultas
                .Where(c => c.ExpedienteId == id)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            var vm = new ExpedienteDetalleViewModel
            {
                Expediente   = expediente,
                Paciente     = expediente.Paciente,
                Alergias     = alergias,
                Enfermedades = enfermedades,
                Antecedentes = antecedentes,
                Consultas    = consultas,
                TabActiva    = tab
            };

            return View(vm);
        }

        // GET: Expedientes/Create — Registro unificado nuevo paciente
        public IActionResult Create()
        {
            return View(new PacienteExpedienteRegistroViewModel());
        }

        // POST: Expedientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PacienteExpedienteRegistroViewModel vm)
        {
            // Calcular edad
            int edad = 0;
            if (vm.FechaNac.HasValue)
            {
                var hoy = DateTime.Today;
                edad = hoy.Year - vm.FechaNac.Value.Year;
                if (vm.FechaNac.Value.Date > hoy.AddYears(-edad)) edad--;
            }
            bool esMenor = edad < 18;

            // Validaciones condicionales
            if (!esMenor && string.IsNullOrWhiteSpace(vm.DUI))
                ModelState.AddModelError("DUI", "El DUI es obligatorio para pacientes mayores de edad.");

            if (esMenor)
            {
                var responsablesDtos = ParseJson<List<ResponsableDto>>(vm.ResponsablesJson);
                if (responsablesDtos == null || responsablesDtos.Count == 0)
                    ModelState.AddModelError("ResponsablesJson", "Debe agregar al menos un responsable para pacientes menores de edad.");
            }

            // Generar código automático si viene vacío
            if (string.IsNullOrWhiteSpace(vm.Codigo))
            {
                var total = await _context.Pacientes.CountAsync();
                vm.Codigo = $"PAC-{(total + 1):D4}";
            }

            if (!ModelState.IsValid)
                return View(vm);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Crear Paciente
                var paciente = new Pacientes
                {
                    Codigo    = vm.Codigo,
                    Nombre    = vm.Nombre,
                    Sexo      = vm.Sexo,
                    FechaNac  = vm.FechaNac,
                    DUI       = esMenor ? null : vm.DUI,
                    Altura    = vm.Altura,
                    Peso      = vm.Peso,
                    Telefono  = vm.Telefono,
                    Direccion = vm.Direccion,
                    CreadoEn  = DateTime.Now
                };
                _context.Pacientes.Add(paciente);
                await _context.SaveChangesAsync();

                // 2. Crear Expediente
                var expediente = new Expedientes
                {
                    PacienteId    = paciente.PacienteId,
                    Codigo        = $"EXP-{paciente.Codigo}",
                    Historial     = vm.Historial ?? "Sin historial previo.",
                    FechaCreacion = DateTime.Now
                };
                _context.Expedientes.Add(expediente);
                await _context.SaveChangesAsync();

                // 3a. Contactos de Emergencia (pacientes mayores)
                if (!esMenor)
                {
                    var contactosDtos = ParseJson<List<ContactoEmergenciaDto>>(vm.ContactosJson);
                    if (contactosDtos != null)
                    {
                        foreach (var dto in contactosDtos)
                        {
                            _context.ContactoEmergencias.Add(new ContactoEmergencia
                            {
                                PacienteId      = paciente.PacienteId,
                                Nombre          = dto.Nombre,
                                Relacion        = dto.Relacion,
                                Telefono        = dto.Telefono,
                                TelefonoAlterno = dto.TelefonoAlterno ?? "",
                                Direccion       = dto.Direccion ?? "",
                                Prioridad       = dto.Prioridad,
                                Activo          = true,
                                CreadoEn        = DateTime.Now
                            });
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                // 3b. Responsables (pacientes menores)
                else
                {
                    var responsablesDtos = ParseJson<List<ResponsableDto>>(vm.ResponsablesJson);
                    if (responsablesDtos != null)
                    {
                        foreach (var dto in responsablesDtos)
                        {
                            _context.Responsables.Add(new Responsables
                            {
                                PacienteId = paciente.PacienteId,
                                Nombre     = dto.Nombre,
                                Parentesco = dto.Parentesco,
                                DUI        = dto.DUI,
                                Telefono   = dto.Telefono,
                                Direccion  = dto.Direccion
                            });
                        }
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();
                TempData["Success"] = $"Expediente {expediente.Codigo} creado exitosamente para '{paciente.Nombre}'.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Ocurrió un error al guardar. Por favor intente de nuevo.");
                return View(vm);
            }
        }

        // GET: Expedientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var expediente = await _context.Expedientes
                .Include(e => e.Paciente)
                .FirstOrDefaultAsync(e => e.ExpedienteId == id);
            if (expediente == null) return NotFound();

            return View(expediente);
        }

        // POST: Expedientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("ExpedienteId,PacienteId,Codigo,Historial,FechaCreacion")] Expedientes expediente,
            // Datos del paciente
            string? PNombre, string? PSexo, DateTime? PFechaNac, string? PDUI,
            decimal? PAltura, decimal? PPeso, string? PTelefono, string? PDireccion)
        {
            if (id != expediente.ExpedienteId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Guardar expediente
                    _context.Update(expediente);

                    // Guardar datos del paciente si existen
                    var paciente = await _context.Pacientes.FindAsync(expediente.PacienteId);
                    if (paciente != null)
                    {
                        if (!string.IsNullOrWhiteSpace(PNombre))    paciente.Nombre    = PNombre;
                        if (!string.IsNullOrWhiteSpace(PSexo))      paciente.Sexo      = PSexo;
                        if (PFechaNac.HasValue)                      paciente.FechaNac  = PFechaNac;
                        paciente.DUI       = PDUI;
                        if (PAltura.HasValue)                        paciente.Altura    = PAltura;
                        if (PPeso.HasValue)                          paciente.Peso      = PPeso;
                        paciente.Telefono  = PTelefono;
                        paciente.Direccion = PDireccion;
                        _context.Update(paciente);
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Expediente y datos del paciente actualizados correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpedienteExists(expediente.ExpedienteId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Details), new { id = expediente.ExpedienteId });
            }

            // Recargar paciente si hay errores
            expediente.Paciente = await _context.Pacientes.FindAsync(expediente.PacienteId);
            return View(expediente);
        }

        // Eliminar bloqueado
        public IActionResult Delete(int? id)
        {
            TempData["Error"] = "Los expedientes clínicos son registros médicos permanentes y no se pueden eliminar.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            TempData["Error"] = "Los expedientes clínicos son registros médicos permanentes y no se pueden eliminar.";
            return RedirectToAction(nameof(Index));
        }

        private bool ExpedienteExists(int id)
            => _context.Expedientes.Any(e => e.ExpedienteId == id);

        private static T? ParseJson<T>(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return default;
            try { return System.Text.Json.JsonSerializer.Deserialize<T>(json); }
            catch { return default; }
        }
    }
}
