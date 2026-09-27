using System.ComponentModel.DataAnnotations;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class Alergias
    {
        public int AlergiaId { get; set; }

        [Required(ErrorMessage = "El nombre de la alergia es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        [StringLength(50)]
        public string Grado { get; set; } = string.Empty;
    }
}
