using System.ComponentModel.DataAnnotations;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class Antecedentes
    {
        public int AntecedentesId { get; set; }

        [Required(ErrorMessage = "El tipo de antecedente es obligatorio")]
        [StringLength(100)]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        public DateTime? Fecha { get; set; }
    }
}
