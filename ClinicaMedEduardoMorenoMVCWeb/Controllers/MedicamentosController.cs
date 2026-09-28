using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using ClinicaMedEduardoMorenoMVCWeb.Data;

namespace ClinicaMedEduardoMorenoMVCWeb.Controllers
{
    public class MedicamentosController : Controller
    {
        private readonly AppDbContext _context;

        public MedicamentosController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int pagina = 1, string? buscar = null)
        {
            if (pagina < 1) pagina = 1;
            int tamañoPagina = 10;

            // Búsqueda por código o nombre
            var query = _context.Medicamentos.AsQueryable();
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();
                query = query.Where(m => m.Codigo.Contains(buscar) || m.Nombre.Contains(buscar));
            }

            var totalRegistros = query.Count();
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamañoPagina);
            if (totalPaginas == 0) totalPaginas = 1;

            if (pagina > totalPaginas) pagina = totalPaginas;

            var medicamentos = query
                .OrderBy(m => m.MedicamentoId)
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .ToList();

            ViewBag.Buscar = buscar;
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.TamañoPagina = tamañoPagina;

            return View(medicamentos);
        }

        private string GenerarSiguienteCodigo()
        {
            var codigos = _context.Medicamentos.Select(m => m.Codigo).ToList();
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
            return $"MED-{siguienteNumero:D3}";
        }

        public IActionResult Create()
        {
            var medicamento = new Medicamentos
            {
                Codigo = GenerarSiguienteCodigo()
            };
            return View(medicamento);
        }

        [HttpPost]
        public IActionResult Create(Medicamentos medicamento)
        {
            // Asignar el código automáticamente para garantizar consistencia y correlatividad
            medicamento.Codigo = GenerarSiguienteCodigo();
            ModelState.Remove(nameof(medicamento.Codigo));

            if (ModelState.IsValid)
            {
                _context.Medicamentos.Add(medicamento);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(medicamento);
        }

        public IActionResult Details(int id)
        {
            var medicamento = _context.Medicamentos
                .FirstOrDefault(m => m.MedicamentoId == id);

            if (medicamento == null)
            {
                return NotFound();
            }

            return View(medicamento);
        }

        public IActionResult Edit(int id)
        {
            var medicamento = _context.Medicamentos.Find(id);

            if (medicamento == null)
            {
                return NotFound();
            }

            return View(medicamento);
        }

        [HttpPost]
        public IActionResult Edit(int id, Medicamentos medicamento)
        {
            if (id != medicamento.MedicamentoId)
            {
                return NotFound();
            }

            var medExistente = _context.Medicamentos.AsNoTracking().FirstOrDefault(m => m.MedicamentoId == id);
            if (medExistente == null)
            {
                return NotFound();
            }

            // El código es fijo y no debe modificarse
            medicamento.Codigo = medExistente.Codigo;
            ModelState.Remove(nameof(medicamento.Codigo));

            if (ModelState.IsValid)
            {
                _context.Update(medicamento);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(medicamento);
        }

        public IActionResult Delete(int id)
        {
            var medicamento = _context.Medicamentos
                .FirstOrDefault(m => m.MedicamentoId == id);

            if (medicamento == null)
            {
                return NotFound();
            }

            return View(medicamento);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var medicamento = _context.Medicamentos.Find(id);

            if (medicamento != null)
            {
                _context.Medicamentos.Remove(medicamento);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
