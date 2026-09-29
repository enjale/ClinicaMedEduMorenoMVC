using System.ComponentModel.DataAnnotations;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class Recetas
    {
        [Key]
        public int RecetaId { get; set; }

        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La consulta es obligatoria.")]
        [Display(Name = "Consulta")]
        public int ConsultaId { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [StringLength(100)]
        [Display(Name = "Médico")]
        public string? Medico { get; set; }
    }
}
