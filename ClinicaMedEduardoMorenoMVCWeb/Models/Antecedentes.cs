using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class Antecedentes
    {
        [Key]
        [Column("AntecedenteId")]
        public int AntecedentesId { get; set; }

        [StringLength(20)]
        [Display(Name = "Código")]
        public string? Codigo { get; set; }

        [Required(ErrorMessage = "El tipo de antecedente es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Tipo de antecedente")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [Display(Name = "Fecha")]
        public DateTime? Fecha { get; set; }

        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        // Vínculo con Expediente (nullable para no romper registros existentes)
        [Display(Name = "Expediente")]
        public int? ExpedienteId { get; set; }

        [ForeignKey("ExpedienteId")]
        public virtual Expedientes? Expediente { get; set; }
    }
}
