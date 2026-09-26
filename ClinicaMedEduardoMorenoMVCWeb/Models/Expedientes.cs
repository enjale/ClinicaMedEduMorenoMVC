using System.ComponentModel.DataAnnotations;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class Expedientes
    {
        public int ExpedienteId { get; set; }

        [Required(ErrorMessage = "El paciente es obligatorio")]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(100)]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El historial es obligatorio")]
        [StringLength(500)]
        public string Historial { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de creación es obligatoria")]
        public DateTime FechaCreacion { get; set; }
    }
}
