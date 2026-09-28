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
            return View();
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
            return View();
        }

        public IActionResult Edit(int id)
        {
            return View();
        }

        public IActionResult Delete(int id)
        {
            return View();
        }
    }
}

