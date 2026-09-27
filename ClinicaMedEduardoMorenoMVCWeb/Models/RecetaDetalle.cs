using System.ComponentModel.DataAnnotations;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class RecetaDetalle
    {
        public int RecetaDetalleId { get; set; }

        [Required(ErrorMessage = "El id de la receta es obligatorio")]
        [Display(Name = "Receta")]
        public int RecetaId { get; set; }

        [Required(ErrorMessage = "El id del medicamento es obligatorio")]
        [Display(Name = "Medicamento")]
        public int MedicamentoId { get; set; }

        [Required(ErrorMessage = "El orden es obligatorio")]
        public byte Orden { get; set; } = 1;

        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(200)]
        public string Prescripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [StringLength(100)]
        public string Cantidad { get; set; } = string.Empty;
    }
}
