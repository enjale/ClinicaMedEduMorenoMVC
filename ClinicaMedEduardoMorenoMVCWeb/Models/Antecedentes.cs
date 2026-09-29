using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class Antecedentes
    {
        [Key]
        public int AntecedentesId { get; set; }

        [Required(ErrorMessage = "El tipo de antecedente es obligatorio")]
        [StringLength(100)]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        public DateTime? Fecha { get; set; }

        // Vínculo con Expediente (nullable para no romper registros existentes)
        public int? ExpedienteId { get; set; }

        [ForeignKey("ExpedienteId")]
        public virtual Expedientes? Expediente { get; set; }
    }
}
