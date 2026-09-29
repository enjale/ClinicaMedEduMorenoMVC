using System.ComponentModel.DataAnnotations;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class Alergias
    {
        [Key]
        public int AlergiaId { get; set; }

        [Required(ErrorMessage = "El nombre de la alergia es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string Codigo { get; set; } = string.Empty;
    }
}
