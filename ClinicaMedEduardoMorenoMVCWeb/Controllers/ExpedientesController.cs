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

        // GET: Expedientes (con filtros de búsqueda)
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

            // Guardar filtros para la vista
            ViewBag.Buscar = buscar;
            ViewBag.Sexo = sexo ?? "Todos";
            ViewBag.CreadoDesde = creadoDesde?.ToString("yyyy-MM-dd");
            ViewBag.CreadoHasta = creadoHasta?.ToString("yyyy-MM-dd");

            var lista = await query.OrderByDescending(e => e.FechaCreacion).ToListAsync();
            return View(lista);
        }

        // GET: Expedientes/Details/5?tab=alergias
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

            var vm = new ExpedienteDetalleViewModel
            {
                Expediente = expediente,
                Paciente = expediente.Paciente,
                Alergias = alergias,
                Enfermedades = enfermedades,
                Antecedentes = antecedentes,
                TabActiva = tab
            };

            return View(vm);
        }

        // GET: Expedientes/Create
        public IActionResult Create()
        {
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre");
            return View();
        }

        // POST: Expedientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExpedienteId,PacienteId,Codigo,Historial,FechaCreacion")] Expedientes expediente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expediente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre", expediente.PacienteId);
            return View(expediente);
        }

        // GET: Expedientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var expediente = await _context.Expedientes.FindAsync(id);
            if (expediente == null) return NotFound();

            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre", expediente.PacienteId);
            return View(expediente);
        }

        // POST: Expedientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExpedienteId,PacienteId,Codigo,Historial,FechaCreacion")] Expedientes expediente)
        {
            if (id != expediente.ExpedienteId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expediente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpedienteExists(expediente.ExpedienteId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre", expediente.PacienteId);
            return View(expediente);
        }

        // GET: Expedientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var expediente = await _context.Expedientes
                .Include(e => e.Paciente)
                .FirstOrDefaultAsync(m => m.ExpedienteId == id);

            if (expediente == null) return NotFound();
            return View(expediente);
        }

        // POST: Expedientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expediente = await _context.Expedientes.FindAsync(id);
            if (expediente != null) _context.Expedientes.Remove(expediente);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpedienteExists(int id)
        {
            return _context.Expedientes.Any(e => e.ExpedienteId == id);
        }
    }
}
