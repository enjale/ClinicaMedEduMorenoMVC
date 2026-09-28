using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using ClinicaMedEduardoMorenoMVCWeb.Data;

namespace ClinicaMedEduardoMorenoMVCWeb.Controllers
{
    public class AlergiasController : Controller
    {
        private readonly AppDbContext _context;

        public AlergiasController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int pagina = 1, string? buscar = null)
        {
            if (pagina < 1) pagina = 1;
            int tamañoPagina = 10;

            // Búsqueda por código o nombre
            var query = _context.Alergias.AsQueryable();
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();
                query = query.Where(a => a.Codigo.Contains(buscar) || a.Nombre.Contains(buscar));
            }

            var totalRegistros = query.Count();
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamañoPagina);
            if (totalPaginas == 0) totalPaginas = 1;

            if (pagina > totalPaginas) pagina = totalPaginas;

            var alergias = query
                .OrderByDescending(a => a.AlergiaId)
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .ToList();

            ViewBag.Buscar = buscar;
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.TamañoPagina = tamañoPagina;

            return View(alergias);
        }

        private string GenerarSiguienteCodigo()
        {
            var codigos = _context.Alergias.Select(a => a.Codigo).ToList();
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
            return $"ALER-{siguienteNumero:D3}";
        }

        public IActionResult Create()
        {
            var alergia = new Alergias
            {
                Codigo = GenerarSiguienteCodigo()
            };
            return View(alergia);
        }

        [HttpPost]
        public IActionResult Create(Alergias alergia)
        {
            // Asignar el código automáticamente para garantizar consistencia y correlatividad
            alergia.Codigo = GenerarSiguienteCodigo();
            ModelState.Remove(nameof(alergia.Codigo));

            if (ModelState.IsValid)
            {
                _context.Alergias.Add(alergia);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(alergia);
        }

        public IActionResult Details(int id)
        {
            var alergia = _context.Alergias
                .FirstOrDefault(a => a.AlergiaId == id);

            if (alergia == null)
            {
                return NotFound();
            }

            return View(alergia);
        }

        public IActionResult Edit(int id)
        {
            var alergia = _context.Alergias.Find(id);

            if (alergia == null)
            {
                return NotFound();
            }

            return View(alergia);
        }

        [HttpPost]
        public IActionResult Edit(int id, Alergias alergia)
        {
            if (id != alergia.AlergiaId)
            {
                return NotFound();
            }

            var alergiaExistente = _context.Alergias.AsNoTracking().FirstOrDefault(a => a.AlergiaId == id);
            if (alergiaExistente == null)
            {
                return NotFound();
            }

            // El código es fijo y no debe modificarse
            alergia.Codigo = alergiaExistente.Codigo;
            ModelState.Remove(nameof(alergia.Codigo));

            if (ModelState.IsValid)
            {
                _context.Update(alergia);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(alergia);
        }

        public IActionResult Delete(int id)
        {
            var alergia = _context.Alergias
                .FirstOrDefault(a => a.AlergiaId == id);

            if (alergia == null)
            {
                return NotFound();
            }

            return View(alergia);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var alergia = _context.Alergias.Find(id);

            if (alergia != null)
            {
                _context.Alergias.Remove(alergia);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
