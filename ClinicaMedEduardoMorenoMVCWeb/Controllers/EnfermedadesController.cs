using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using ClinicaMedEduardoMorenoMVCWeb.Data;

namespace ClinicaMedEduardoMorenoMVCWeb.Controllers
{
    public class EnfermedadesController : Controller
    {
        private readonly AppDbContext _context;

        // Valores permitidos por el CHECK constraint CK_Enf_Tipo de la base de datos
        private static readonly string[] TiposEnfermedad = { "Crónica", "Aguda", "Infecciosa", "Congénita", "Otra" };

        public EnfermedadesController(AppDbContext context)
        {
            _context = context;
        }

        private void ValidarTipoEnfermedad(Enfermedades enfermedad)
        {
            if (!string.IsNullOrWhiteSpace(enfermedad.TipoEnfermedad) && !TiposEnfermedad.Contains(enfermedad.TipoEnfermedad))
            {
                ModelState.AddModelError(nameof(enfermedad.TipoEnfermedad), "Seleccione un tipo de enfermedad válido");
            }
        }

        public IActionResult Index(int pagina = 1, string? buscar = null)
        {
            if (pagina < 1) pagina = 1;
            int tamañoPagina = 10;

            // Búsqueda por código o nombre
            var query = _context.Enfermedades.AsQueryable();
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();
                query = query.Where(e => e.Codigo.Contains(buscar) || e.Nombre.Contains(buscar));
            }

            var totalRegistros = query.Count();
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamañoPagina);
            if (totalPaginas == 0) totalPaginas = 1;

            if (pagina > totalPaginas) pagina = totalPaginas;

            var enfermedades = query
                .OrderByDescending(e => e.EnfermedadId)
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .ToList();

            ViewBag.Buscar = buscar;
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.TamañoPagina = tamañoPagina;

            return View(enfermedades);
        }

        private string GenerarSiguienteCodigo()
        {
            var codigos = _context.Enfermedades.Select(e => e.Codigo).ToList();
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
            return $"ENF-{siguienteNumero:D3}";
        }

        public IActionResult Create()
        {
            var enfermedad = new Enfermedades
            {
                Codigo = GenerarSiguienteCodigo()
            };
            ViewBag.TiposEnfermedad = TiposEnfermedad;
            return View(enfermedad);
        }

        [HttpPost]
        public IActionResult Create(Enfermedades enfermedad)
        {
            // Asignar el código automáticamente para garantizar consistencia y correlatividad
            enfermedad.Codigo = GenerarSiguienteCodigo();
            ModelState.Remove(nameof(enfermedad.Codigo));
            ValidarTipoEnfermedad(enfermedad);

            if (ModelState.IsValid)
            {
                _context.Enfermedades.Add(enfermedad);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.TiposEnfermedad = TiposEnfermedad;
            return View(enfermedad);
        }

        public IActionResult Details(int id)
        {
            var enfermedad = _context.Enfermedades
                .FirstOrDefault(e => e.EnfermedadId == id);

            if (enfermedad == null)
            {
                return NotFound();
            }

            return View(enfermedad);
        }

        public IActionResult Edit(int id)
        {
            var enfermedad = _context.Enfermedades.Find(id);

            if (enfermedad == null)
            {
                return NotFound();
            }

            ViewBag.TiposEnfermedad = TiposEnfermedad;
            return View(enfermedad);
        }

        [HttpPost]
        public IActionResult Edit(int id, Enfermedades enfermedad)
        {
            if (id != enfermedad.EnfermedadId)
            {
                return NotFound();
            }

            var enfExistente = _context.Enfermedades.AsNoTracking().FirstOrDefault(e => e.EnfermedadId == id);
            if (enfExistente == null)
            {
                return NotFound();
            }

            // El código es fijo y no debe modificarse
            enfermedad.Codigo = enfExistente.Codigo;
            ModelState.Remove(nameof(enfermedad.Codigo));
            ValidarTipoEnfermedad(enfermedad);

            if (ModelState.IsValid)
            {
                _context.Update(enfermedad);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.TiposEnfermedad = TiposEnfermedad;
            return View(enfermedad);
        }

        public IActionResult Delete(int id)
        {
            var enfermedad = _context.Enfermedades
                .FirstOrDefault(e => e.EnfermedadId == id);

            if (enfermedad == null)
            {
                return NotFound();
            }

            return View(enfermedad);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var enfermedad = _context.Enfermedades.Find(id);

            if (enfermedad != null)
            {
                _context.Enfermedades.Remove(enfermedad);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
