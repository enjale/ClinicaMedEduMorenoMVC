using ClinicaMedEduardoMorenoMVCWeb.Models;
using ClinicaMedEduardoMorenoMVCWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ClinicaMedEduardoMorenoMVCWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }

        public IActionResult DoctorDashboard(string? doctorNombre)
        {
            var model = new DoctorDashboardViewModel
            {
                NombreMedico = string.IsNullOrEmpty(doctorNombre) ? "Dr. Eduardo Moreno" : doctorNombre
            };
            return View(model);
        }

        public IActionResult EnfermeraDashboard(string? enfermera)
        {
            var model = new EnfermeraDashboardViewModel
            {
                NombreEnfermera = string.IsNullOrEmpty(enfermera) ? "Enfermera" : enfermera
            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
