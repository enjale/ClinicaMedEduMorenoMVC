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

        public IActionResult Index()
        {
            var medicamentos = _context.Medicamentos.ToList();
            return View(medicamentos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Medicamentos medicamento)
        {
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
