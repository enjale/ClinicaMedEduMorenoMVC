using System.ComponentModel.DataAnnotations;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class Responsables
    {
        [Key]
        public int ResponsableId { get; set; }

        [Required(ErrorMessage = "El paciente es obligatorio.")]
        [Display(Name = "Paciente")]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "El DUI no puede tener más de 10 caracteres.")]
        [RegularExpression(@"^\d{8}-\d{1}$", ErrorMessage = "El formato del DUI no es válido (ej. 12345678-9)")]
        public string? DUI { get; set; }

        [Required(ErrorMessage = "El parentesco es obligatorio.")]
        [StringLength(50)]
        public string Parentesco { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Teléfono")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        public string? Telefono { get; set; }

        [StringLength(250)]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }
    }
}
