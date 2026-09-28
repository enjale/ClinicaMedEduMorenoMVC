using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using ClinicaMedEduardoMorenoMVCWeb.Data;

namespace ClinicaMedEduardoMorenoMVCWeb.Controllers
{
    public class RecetasController : Controller
    {
        private readonly AppDbContext _context;

        public RecetasController(AppDbContext context) 
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var recetas = _context.Recetas.ToList();
            return View(recetas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Recetas receta)
        {
            if (ModelState.IsValid)
            {
                _context.Recetas.Add(receta);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(receta);
        }

        public IActionResult Details(int id)
        {
            var receta = _context.Recetas
                .FirstOrDefault(r => r.RecetaId == id);

            if (receta == null)
            {
                return NotFound();
            }

            return View(receta);
        }

        public IActionResult Edit(int id)
        {
            var receta = _context.Recetas.Find(id);

            if (receta == null)
            {
                return NotFound();
            }

            return View(receta);
        }

        [HttpPost]
        public IActionResult Edit(int id, Recetas receta)
        {
            if (id != receta.RecetaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(receta);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(receta);
        }

        public IActionResult Delete(int id)
        {
            var receta = _context.Recetas
                .FirstOrDefault(r => r.RecetaId == id);

            if (receta == null)
            {
                return NotFound();
            }

            return View(receta);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var receta = _context.Recetas.Find(id);

            if (receta != null)
            {
                _context.Recetas.Remove(receta);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
