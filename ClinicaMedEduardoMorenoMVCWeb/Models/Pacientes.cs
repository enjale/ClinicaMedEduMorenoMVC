using System.ComponentModel.DataAnnotations;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class Pacientes
    {
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El sexo es obligatorio")]
        [StringLength(1)]
        public string Sexo { get; set; } = string.Empty;

        public DateTime? FechaNac { get; set; }
        public string? DUI { get; set; }
        public decimal? Altura { get; set; }
        public decimal? Peso { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }

        [Required(ErrorMessage = "La fecha de creación es obligatoria")]
        public DateTime CreadoEn { get; set; }
    }
}
