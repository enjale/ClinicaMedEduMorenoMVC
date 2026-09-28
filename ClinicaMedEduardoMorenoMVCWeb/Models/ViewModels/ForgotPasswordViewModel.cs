using System.ComponentModel.DataAnnotations;

namespace ClinicaMedEduardoMorenoMVCWeb.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Por favor ingrese su correo electrónico o nombre de usuario.")]
        [Display(Name = "Correo Electrónico o Usuario")]
        public string EmailOrUsername { get; set; } = string.Empty;
    }
}
