using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using ClinicaMedEduardoMorenoMVCWeb.Data;

namespace ClinicaMedEduardoMorenoMVCWeb.Controllers
{
    public class AntecedentesController : Controller
    {
        private readonly AppDbContext _context;

        public AntecedentesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Antecedentes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Antecedentes.ToListAsync());
        }

        // GET: Antecedentes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var antecedente = await _context.Antecedentes
                .FirstOrDefaultAsync(m => m.AntecedentesId == id);

            if (antecedente == null) return NotFound();
            return View(antecedente);
        }

        private string GenerarSiguienteCodigo()
        {
            var codigos = _context.Antecedentes.Select(a => a.Codigo).ToList();
            int maxNumero = 0;

            foreach (var cod in codigos)
            {
                if (string.IsNullOrWhiteSpace(cod)) continue;
                var match = System.Text.RegularExpressions.Regex.Match(cod, @"\d+");
                if (match.Success && int.TryParse(match.Value, out int num))
                {
                    if (num > maxNumero) maxNumero = num;
                }
            }

            int siguienteNumero = maxNumero + 1;
            return $"ANT-{siguienteNumero:D3}";
        }

        // GET: Antecedentes/Create?expedienteId=5
        public IActionResult Create(int? expedienteId)
        {
            ViewBag.ExpedienteIdActual = expedienteId;
            return View(new Antecedentes { 
                ExpedienteId = expedienteId,
                Codigo = GenerarSiguienteCodigo()
            });
        }

        // POST: Antecedentes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AntecedentesId,Codigo,Tipo,Descripcion,Fecha,ExpedienteId")] Antecedentes antecedente)
        {
            antecedente.Codigo = GenerarSiguienteCodigo();
            ModelState.Remove(nameof(antecedente.Codigo));

            if (ModelState.IsValid)
            {
                _context.Add(antecedente);
                await _context.SaveChangesAsync();

                // Si viene de un expediente, volver a él con tab de antecedentes
                if (antecedente.ExpedienteId.HasValue)
                    return RedirectToAction("Details", "Expedientes",
                        new { id = antecedente.ExpedienteId.Value, tab = "antecedentes" });

                return RedirectToAction(nameof(Index));
            }
            ViewBag.ExpedienteIdActual = antecedente.ExpedienteId;
            return View(antecedente);
        }

        // GET: Antecedentes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var antecedente = await _context.Antecedentes.FindAsync(id);
            if (antecedente == null) return NotFound();

            return View(antecedente);
        }

        // POST: Antecedentes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AntecedentesId,Codigo,Tipo,Descripcion,Fecha,ExpedienteId")] Antecedentes antecedente)
        {
            if (id != antecedente.AntecedentesId) return NotFound();

            if (string.IsNullOrWhiteSpace(antecedente.Codigo))
            {
                var antExistente = await _context.Antecedentes.AsNoTracking().FirstOrDefaultAsync(a => a.AntecedentesId == id);
                antecedente.Codigo = antExistente?.Codigo ?? GenerarSiguienteCodigo();
            }
            ModelState.Remove(nameof(antecedente.Codigo));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(antecedente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Antecedentes.Any(e => e.AntecedentesId == id)) return NotFound();
                    else throw;
                }

                if (antecedente.ExpedienteId.HasValue)
                    return RedirectToAction("Details", "Expedientes",
                        new { id = antecedente.ExpedienteId.Value, tab = "antecedentes" });

                return RedirectToAction(nameof(Index));
            }
            return View(antecedente);
        }

        // POST: Eliminar antecedente desde la vista de expediente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFromExpediente(int id, int expedienteId)
        {
            var antecedente = await _context.Antecedentes.FindAsync(id);
            if (antecedente != null)
                _context.Antecedentes.Remove(antecedente);

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Expedientes",
                new { id = expedienteId, tab = "antecedentes" });
        }

        // GET: Antecedentes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var antecedente = await _context.Antecedentes
                .FirstOrDefaultAsync(m => m.AntecedentesId == id);

            if (antecedente == null) return NotFound();
            return View(antecedente);
        }

        // POST: Antecedentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var antecedente = await _context.Antecedentes.FindAsync(id);
            int? expedienteId = antecedente?.ExpedienteId;
            if (antecedente != null) _context.Antecedentes.Remove(antecedente);

            await _context.SaveChangesAsync();

            if (expedienteId.HasValue)
                return RedirectToAction("Details", "Expedientes",
                    new { id = expedienteId.Value, tab = "antecedentes" });

            return RedirectToAction(nameof(Index));
        }

        private bool AntecedenteExists(int id)
        {
            return _context.Antecedentes.Any(e => e.AntecedentesId == id);
        }
    }
}
