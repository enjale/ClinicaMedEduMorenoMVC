using ClinicaMedEduardoMorenoMVCWeb.Models;
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

        public IActionResult DoctorDashboard()
        {
            return View();
        }

        public IActionResult EnfermeraDashboard()
        {
            return View();
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
