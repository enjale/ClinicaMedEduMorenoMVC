using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using ClinicaMedEduardoMorenoMVCWeb.Data;

namespace ClinicaMedEduardoMorenoMVCWeb.Controllers
{
    public class ExpedienteAlergiasController : Controller
    {
        private readonly AppDbContext _context;

        public ExpedienteAlergiasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ExpedienteAlergias
        public async Task<IActionResult> Index()
        {
            var lista = await _context.ExpedienteAlergias
                .Include(e => e.Expediente)
                .Include(e => e.Alergia)
                .ToListAsync();
            return View(lista);
        }

        // GET: ExpedienteAlergias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.ExpedienteAlergias
                .Include(e => e.Expediente)
                .Include(e => e.Alergia)
                .FirstOrDefaultAsync(m => m.ExpedienteAlergiaId == id);

            if (item == null) return NotFound();
            return View(item);
        }

        // GET: ExpedienteAlergias/Create?expedienteId=5
        public IActionResult Create(int? expedienteId)
        {
            ViewData["ExpedienteId"] = new SelectList(_context.Expedientes, "ExpedienteId", "Codigo", expedienteId);
            ViewData["AlergiaId"] = new SelectList(_context.Alergias, "AlergiaId", "Nombre");
            ViewBag.ExpedienteIdActual = expedienteId;
            return View();
        }

        // POST: ExpedienteAlergias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExpedienteAlergiaId,ExpedienteId,AlergiaId,Nivel,Observaciones")] ExpedienteAlergias item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                // Redirigir al expediente con tab de alergias
                return RedirectToAction("Details", "Expedientes",
                    new { id = item.ExpedienteId, tab = "alergias" });
            }
            ViewData["ExpedienteId"] = new SelectList(_context.Expedientes, "ExpedienteId", "Codigo", item.ExpedienteId);
            ViewData["AlergiaId"] = new SelectList(_context.Alergias, "AlergiaId", "Nombre", item.AlergiaId);
            ViewBag.ExpedienteIdActual = item.ExpedienteId;
            return View(item);
        }

        // GET: ExpedienteAlergias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.ExpedienteAlergias.FindAsync(id);
            if (item == null) return NotFound();

            ViewData["ExpedienteId"] = new SelectList(_context.Expedientes, "ExpedienteId", "Codigo", item.ExpedienteId);
            ViewData["AlergiaId"] = new SelectList(_context.Alergias, "AlergiaId", "Nombre", item.AlergiaId);
            ViewBag.ExpedienteIdActual = item.ExpedienteId;
            return View(item);
        }

        // POST: ExpedienteAlergias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExpedienteAlergiaId,ExpedienteId,AlergiaId,Nivel,Observaciones")] ExpedienteAlergias item)
        {
            if (id != item.ExpedienteAlergiaId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ExpedienteAlergias.Any(e => e.ExpedienteAlergiaId == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction("Details", "Expedientes",
                    new { id = item.ExpedienteId, tab = "alergias" });
            }
            ViewData["ExpedienteId"] = new SelectList(_context.Expedientes, "ExpedienteId", "Codigo", item.ExpedienteId);
            ViewData["AlergiaId"] = new SelectList(_context.Alergias, "AlergiaId", "Nombre", item.AlergiaId);
            return View(item);
        }

        // POST: ExpedienteAlergias/DeleteFromExpediente (eliminar desde la vista de expediente)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFromExpediente(int id, int expedienteId)
        {
            var item = await _context.ExpedienteAlergias.FindAsync(id);
            if (item != null)
                _context.ExpedienteAlergias.Remove(item);

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Expedientes",
                new { id = expedienteId, tab = "alergias" });
        }

        // GET: ExpedienteAlergias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.ExpedienteAlergias
                .Include(e => e.Expediente)
                .Include(e => e.Alergia)
                .FirstOrDefaultAsync(m => m.ExpedienteAlergiaId == id);

            if (item == null) return NotFound();
            return View(item);
        }

        // POST: ExpedienteAlergias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.ExpedienteAlergias.FindAsync(id);
            int? expedienteId = item?.ExpedienteId;
            if (item != null) _context.ExpedienteAlergias.Remove(item);

            await _context.SaveChangesAsync();

            if (expedienteId.HasValue)
                return RedirectToAction("Details", "Expedientes",
                    new { id = expedienteId.Value, tab = "alergias" });

            return RedirectToAction(nameof(Index));
        }
    }
}
