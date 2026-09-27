using Microsoft.AspNetCore.Mvc;
using ClinicaMedEduardoMorenoMVCWeb.Data;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class Pacientes
    {
        [Key]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El sexo es obligatorio")]
        [StringLength(1)]
        public string Sexo { get; set; } = string.Empty;

        public DateTime? FechaNac { get; set; }
        
        [RegularExpression(@"^\d{8}-\d{1}$", ErrorMessage = "El formato del DUI no es válido (ej. 12345678-9)")]
        public string? DUI { get; set; }
        
        [Range(0.1, 3.0, ErrorMessage = "Ingrese una altura válida en metros")]
        public decimal? Altura { get; set; }
        
        [Range(0.1, 500.0, ErrorMessage = "Ingrese un peso válido")]
        public decimal? Peso { get; set; }
        
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        public string? Telefono { get; set; }
        
        public string? Direccion { get; set; }

        [Required(ErrorMessage = "La fecha de creación es obligatoria")]
        public DateTime CreadoEn { get; set; }
    }
}
