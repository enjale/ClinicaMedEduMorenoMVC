using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using ClinicaMedEduardoMorenoMVCWeb.Data;

namespace ClinicaMedEduardoMorenoMVCWeb.Controllers
{
    public class RecetaDetalleController : Controller
    {
        private readonly AppDbContext _context;

        public RecetaDetalleController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var detalles = _context.RecetaDetalles.ToList();
            return View(detalles);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(RecetaDetalle recetaDetalle)
        {
            if (ModelState.IsValid)
            {
                _context.RecetaDetalles.Add(recetaDetalle);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(recetaDetalle);
        }

        public IActionResult Details(int id)
        {
            var recetaDetalle = _context.RecetaDetalles
                .FirstOrDefault(rd => rd.RecetaDetalleId == id);

            if (recetaDetalle == null)
            {
                return NotFound();
            }

            return View(recetaDetalle);
        }

        public IActionResult Edit(int id)
        {
            var recetaDetalle = _context.RecetaDetalles.Find(id);

            if (recetaDetalle == null)
            {
                return NotFound();
            }

            return View(recetaDetalle);
        }

        [HttpPost]
        public IActionResult Edit(int id, RecetaDetalle recetaDetalle)
        {
            if (id != recetaDetalle.RecetaDetalleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(recetaDetalle);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(recetaDetalle);
        }

        public IActionResult Delete(int id)
        {
            var recetaDetalle = _context.RecetaDetalles
                .FirstOrDefault(rd => rd.RecetaDetalleId == id);

            if (recetaDetalle == null)
            {
                return NotFound();
            }

            return View(recetaDetalle);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var recetaDetalle = _context.RecetaDetalles.Find(id);

            if (recetaDetalle != null)
            {
                _context.RecetaDetalles.Remove(recetaDetalle);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
