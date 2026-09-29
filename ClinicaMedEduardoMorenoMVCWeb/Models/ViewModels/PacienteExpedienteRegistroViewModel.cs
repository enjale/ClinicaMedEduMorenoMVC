using System.ComponentModel.DataAnnotations;

namespace ClinicaMedEduardoMorenoMVCWeb.Models.ViewModels
{
    public class PacienteExpedienteRegistroViewModel
    {
        public int? PacienteId { get; set; }

        [Display(Name = "Código")]
        public string? Codigo { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(150)]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "DUI")]
        public string? DUI { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio")]
        [StringLength(1)]
        [Display(Name = "Sexo")]
        public string Sexo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime? FechaNac { get; set; }

        [Display(Name = "Altura")]
        public decimal? Altura { get; set; }

        [Display(Name = "Peso (kg)")]
        public decimal? Peso { get; set; }

        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [Display(Name = "Historial Inicial")]
        [StringLength(500)]
        public string? Historial { get; set; }

        // JSON de Contactos de Emergencia (para adultos)
        public string? ContactosJson { get; set; }

        // JSON de Responsables (para menores)
        public string? ResponsablesJson { get; set; }
    }

    public class ContactoEmergenciaDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Relacion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string? TelefonoAlterno { get; set; }
        public string? Direccion { get; set; }
        public int Prioridad { get; set; } = 1;
    }

    public class ResponsableDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Parentesco { get; set; } = string.Empty;
        public string? DUI { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
    }
}
