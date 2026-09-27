using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaMedEduardoMorenoMVCWeb.Data;
using ClinicaMedEduardoMorenoMVCWeb.Models.ViewModels;

namespace ClinicaMedEduardoMorenoMVCWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Buscar usuario en la base de datos por nombre de usuario
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == model.NombreUsuario);

            // Validar existencia y verificar hash de contraseña (PBKDF2 o texto plano)
            if (usuario == null || !ClinicaMedEduardoMorenoMVCWeb.Services.PasswordHelper.VerifyPassword(model.Contrasena, usuario.Contrasena))
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                return View(model);
            }

            if (!usuario.Activo)
            {
                ModelState.AddModelError(string.Empty, "El usuario se encuentra inactivo.");
                return View(model);
            }

            // Redirección según rol: si es Enfermera a su panel, si es Doctor/otro a DoctorDashboard
            if (usuario.Rol != null && usuario.Rol.Equals("Enfermera", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("EnfermeraDashboard", "Home", new { enfermera = usuario.Nombre });
            }

            return RedirectToAction("DoctorDashboard", "Home", new { doctorNombre = usuario.Nombre });
        }
    }
}

