using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using ClinicaMedEduardoMorenoMVCWeb.Data;
using ClinicaMedEduardoMorenoMVCWeb.Models.ViewModels;
using ClinicaMedEduardoMorenoMVCWeb.Services;

namespace ClinicaMedEduardoMorenoMVCWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IDataProtector _protector;

        public AccountController(
            AppDbContext context,
            IEmailService emailService,
            IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
            _emailService = emailService;
            _protector = dataProtectionProvider.CreateProtector("PasswordReset_ClinicaEduardoMoreno");
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
            if (usuario == null || !PasswordHelper.VerifyPassword(model.Contrasena, usuario.Contrasena))
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

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Buscar usuario por correo electrónico o nombre de usuario
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == model.EmailOrUsername || u.NombreUsuario == model.EmailOrUsername);

            if (usuario == null || string.IsNullOrWhiteSpace(usuario.Email))
            {
                // Por seguridad no revelamos si el usuario existe o no, o indicamos que si existe se envió
                // Si no tiene correo configurado, mostramos aviso
                if (usuario != null && string.IsNullOrWhiteSpace(usuario.Email))
                {
                    ModelState.AddModelError(string.Empty, "El usuario encontrado no tiene un correo electrónico configurado en el sistema.");
                    return View(model);
                }

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // Generar token con expiración de 2 horas
            var expiration = DateTime.UtcNow.AddHours(2).Ticks;
            var tokenPayload = $"{usuario.Email}|{expiration}";
            var token = _protector.Protect(tokenPayload);

            // Generar enlace de restablecimiento
            var resetLink = Url.Action("ResetPassword", "Account", new { token = token, email = usuario.Email }, Request.Scheme);

            try
            {
                await _emailService.SendPasswordResetEmailAsync(usuario.Email, usuario.Nombre, resetLink ?? string.Empty);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al enviar el correo: {ex.Message}");
                return View(model);
            }

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string? token = null, string? email = null)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return BadRequest("El enlace de restablecimiento de contraseña es inválido o está incompleto.");
            }

            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Validar token y expiración
                var unprotectedPayload = _protector.Unprotect(model.Token);
                var parts = unprotectedPayload.Split('|');

                if (parts.Length != 2)
                {
                    ModelState.AddModelError(string.Empty, "El enlace de restablecimiento no es válido.");
                    return View(model);
                }

                var tokenEmail = parts[0];
                var expirationTicks = long.Parse(parts[1]);

                if (!tokenEmail.Equals(model.Email, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(string.Empty, "El correo electrónico no coincide con el token generado.");
                    return View(model);
                }

                if (DateTime.UtcNow.Ticks > expirationTicks)
                {
                    ModelState.AddModelError(string.Empty, "El enlace de restablecimiento ha expirado. Por favor solicite uno nuevo.");
                    return View(model);
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "El token de restablecimiento es inválido o ha sido alterado.");
                return View(model);
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (usuario == null)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            // Hashear y actualizar contraseña
            usuario.Contrasena = PasswordHelper.HashPassword(model.NewPassword);
            usuario.RequiereCambioContrasena = false;

            _context.Update(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}

