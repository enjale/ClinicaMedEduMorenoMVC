using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class Expedientes
    {
        [Key]
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

        // Propiedad de navegación
        [ForeignKey("PacienteId")]
        public virtual Pacientes? Paciente { get; set; }
    }
}
