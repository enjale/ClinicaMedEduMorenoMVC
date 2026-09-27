using Microsoft.AspNetCore.Mvc;
using ClinicaMedEduardoMorenoMVCWeb.Data;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class Usuarios
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Column("Usuario")]
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(100)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(50)]
        public string Contrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [StringLength(50)]
        public string Rol { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El cargo es obligatorio.")]
        [StringLength(100)]
        public string Cargo { get; set; } = string.Empty;

        [Display(Name = "Activo")]
        public bool Activo { get; set; }

        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [StringLength(150)]
        public string? Email { get; set; }

        [Display(Name = "Requiere Cambio de Contraseña")]
        public bool RequiereCambioContrasena { get; set; }
    }
}
