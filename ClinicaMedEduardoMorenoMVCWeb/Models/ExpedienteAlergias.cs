using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class ExpedienteAlergias
    {
        [Key]
        public int ExpedienteAlergiaId { get; set; }

        [Required(ErrorMessage = "El expediente es obligatorio")]
        public int ExpedienteId { get; set; }

        [Required(ErrorMessage = "La alergia es obligatoria")]
        public int AlergiaId { get; set; }

        [Required(ErrorMessage = "El nivel es obligatorio")]
        [StringLength(100)]
        public string Nivel { get; set; } = string.Empty;

        [StringLength(500)]
        public string Observaciones { get; set; } = string.Empty;

        // Propiedades de navegación
        [ForeignKey("ExpedienteId")]
        public virtual Expedientes? Expediente { get; set; }

        [ForeignKey("AlergiaId")]
        public virtual Alergias? Alergia { get; set; }
    }
}