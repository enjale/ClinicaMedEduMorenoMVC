using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using ClinicaMedEduardoMorenoMVCWeb.Models.ViewModels;
using ClinicaMedEduardoMorenoMVCWeb.Data;
using System.Text.Json;

public class PacientesController : Controller
{
    private readonly AppDbContext _context;

    public PacientesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Pacientes
    public async Task<IActionResult> Index()
    {
        return View(await _context.Pacientes.ToListAsync());
    }

    // GET: Pacientes/Details/5
    public async Task<IActionResult> Details(int? pacienteid)
    {
        if (pacienteid == null) return NotFound();

        var paciente = await _context.Pacientes
            .FirstOrDefaultAsync(m => m.PacienteId == pacienteid);
        if (paciente == null) return NotFound();

        return View(paciente);
    }

    // GET: Pacientes/Create
    public IActionResult Create()
    {
        var vm = new PacienteExpedienteRegistroViewModel
        {
            FechaNac = null
        };
        return View(vm);
    }

    // POST: Pacientes/Create
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
            // Para menores: debe haber al menos un responsable
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

        // --- Guardar todo en una transacción ---
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Guardar Paciente
            var paciente = new Pacientes
            {
                Codigo = vm.Codigo,
                Nombre = vm.Nombre,
                Sexo = vm.Sexo,
                FechaNac = vm.FechaNac,
                DUI = esMenor ? null : vm.DUI,
                Altura = vm.Altura,
                Peso = vm.Peso,
                Telefono = vm.Telefono,
                Direccion = vm.Direccion,
                CreadoEn = DateTime.Now
            };
            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();

            // 2. Crear Expediente automáticamente
            var expediente = new Expedientes
            {
                PacienteId = paciente.PacienteId,
                Codigo = $"EXP-{paciente.Codigo}",
                Historial = "Expediente creado al momento del registro.",
                FechaCreacion = DateTime.Now
            };
            _context.Expedientes.Add(expediente);
            await _context.SaveChangesAsync();

            // 3. Guardar Contactos de Emergencia (mayores) o Responsables (menores)
            if (!esMenor)
            {
                var contactosDtos = ParseJson<List<ContactoEmergenciaDto>>(vm.ContactosJson);
                if (contactosDtos != null)
                {
                    foreach (var dto in contactosDtos)
                    {
                        _context.ContactoEmergencias.Add(new ContactoEmergencia
                        {
                            PacienteId = paciente.PacienteId,
                            Nombre = dto.Nombre,
                            Relacion = dto.Relacion,
                            Telefono = dto.Telefono,
                            TelefonoAlterno = dto.TelefonoAlterno ?? "",
                            Direccion = dto.Direccion ?? "",
                            Prioridad = dto.Prioridad,
                            Activo = true,
                            CreadoEn = DateTime.Now
                        });
                    }
                    await _context.SaveChangesAsync();
                }
            }
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
                            Nombre = dto.Nombre,
                            Parentesco = dto.Parentesco,
                            DUI = dto.DUI,
                            Telefono = dto.Telefono,
                            Direccion = dto.Direccion
                        });
                    }
                    await _context.SaveChangesAsync();
                }
            }

            await transaction.CommitAsync();
            TempData["Success"] = $"Paciente '{paciente.Nombre}' registrado exitosamente con expediente {expediente.Codigo}.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError("", "Ocurrió un error al guardar. Por favor intente de nuevo.");
            return View(vm);
        }
    }

    // GET: Pacientes/Edit/5
    public async Task<IActionResult> Edit(int? pacienteid)
    {
        if (pacienteid == null) return NotFound();

        var paciente = await _context.Pacientes.FindAsync(pacienteid);
        if (paciente == null) return NotFound();

        return View(paciente);
    }

    // POST: Pacientes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? pacienteid, [Bind("PacienteId,Codigo,Nombre,Sexo,FechaNac,DUI,Altura,Peso,Telefono,Direccion,CreadoEn")] Pacientes pacientes)
    {
        if (pacienteid != pacientes.PacienteId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(pacientes);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PacientesExists(pacientes.PacienteId))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(pacientes);
    }

    // GET: Pacientes/Delete/5
    public async Task<IActionResult> Delete(int? pacienteid)
    {
        if (pacienteid == null) return NotFound();

        var paciente = await _context.Pacientes
            .FirstOrDefaultAsync(m => m.PacienteId == pacienteid);
        if (paciente == null) return NotFound();

        return View(paciente);
    }

    // POST: Pacientes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? pacienteid)
    {
        var paciente = await _context.Pacientes.FindAsync(pacienteid);
        if (paciente != null)
            _context.Pacientes.Remove(paciente);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PacientesExists(int? pacienteid)
        => _context.Pacientes.Any(e => e.PacienteId == pacienteid);

    private static T? ParseJson<T>(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return default;
        try { return JsonSerializer.Deserialize<T>(json); }
        catch { return default; }
    }
}
