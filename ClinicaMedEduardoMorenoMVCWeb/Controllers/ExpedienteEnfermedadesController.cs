using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using ClinicaMedEduardoMorenoMVCWeb.Data;

namespace ClinicaMedEduardoMorenoMVCWeb.Controllers
{
    public class ExpedienteEnfermedadesController : Controller 
    {
        private readonly AppDbContext _context;

        public ExpedienteEnfermedadesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var expedienteEnfermedades = _context.ExpedienteEnfermedades.ToList();
            return View(expedienteEnfermedades);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ExpedienteEnfermedades expedienteEnfermedad)
        {
            if (ModelState.IsValid)
            {
                _context.ExpedienteEnfermedades.Add(expedienteEnfermedad);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(expedienteEnfermedad);
        }

        public IActionResult Details(int id)
        {
            var expedienteEnfermedad = _context.ExpedienteEnfermedades
                .FirstOrDefault(e => e.ExpedienteEnfermedadId == id);

            if (expedienteEnfermedad == null)
            {
                return NotFound();
            }

            return View(expedienteEnfermedad);
        }

        public IActionResult Edit(int id)
        {
            var expedienteEnfermedad = _context.ExpedienteEnfermedades.Find(id);

            if (expedienteEnfermedad == null)
            {
                return NotFound();
            }

            return View(expedienteEnfermedad);
        }

        [HttpPost]
        public IActionResult Edit(int id, ExpedienteEnfermedades expedienteEnfermedad)
        {
            if (id != expedienteEnfermedad.ExpedienteEnfermedadId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(expedienteEnfermedad);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(expedienteEnfermedad);
        }

        public IActionResult Delete(int id)
        {
            var expedienteEnfermedad = _context.ExpedienteEnfermedades
                .FirstOrDefault(e => e.ExpedienteEnfermedadId == id);

            if (expedienteEnfermedad == null)
            {
                return NotFound();
            }

            return View(expedienteEnfermedad);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var expedienteEnfermedad = _context.ExpedienteEnfermedades.Find(id);

            if (expedienteEnfermedad != null)
            {
                _context.ExpedienteEnfermedades.Remove(expedienteEnfermedad);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
