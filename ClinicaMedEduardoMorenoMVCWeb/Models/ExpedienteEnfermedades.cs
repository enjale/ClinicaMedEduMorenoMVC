using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class ExpedienteEnfermedades
    {
        [Key]
        public int ExpedienteEnfermedadId { get; set; }

        [Required(ErrorMessage = "El expediente es obligatorio")]
        public int ExpedienteId { get; set; }

        [Required(ErrorMessage = "La enfermedad es obligatoria")]
        public int EnfermedadId { get; set; }

        public DateTime? FechaDeteccion { get; set; }

        [StringLength(500)]
        public string Observaciones { get; set; } = string.Empty;

        // Propiedades de navegación
        [ForeignKey("ExpedienteId")]
        public virtual Expedientes? Expediente { get; set; }

        [ForeignKey("EnfermedadId")]
        public virtual Enfermedades? Enfermedad { get; set; }
    }
}