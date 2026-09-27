using Microsoft.AspNetCore.Mvc;
using ClinicaMedEduardoMorenoMVCWeb.Data;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class Consultas
    {
        [Key]
        public int ConsultaId { get; set; }

        [Required(ErrorMessage = "El expediente es obligatorio")]
        public int ExpedienteId { get; set; }

        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El codigo es obligatorio")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [StringLength(1000)]
        public string Motivo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Diagnostico es obligatorio")]
        [StringLength(100)]
        public string Diagnostico { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Tratamiento es obligatorio")]
        [StringLength(100)]
        public string Tratamiento { get; set; } = string.Empty;

        [StringLength(500)]
        public string Observaciones { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del medico es obligatorio")]
        [StringLength(50)]
        public string Medico { get; set; } = string.Empty;

        [Required(ErrorMessage = "El peso es obligatorio")]
        [Range(0.1, 500, ErrorMessage = "ingrese un peso valido")]
        public decimal VitalPeso { get; set; }

        [Required(ErrorMessage = "La temperatura es obligatoria")]
        [Range(30, 45, ErrorMessage = "Ingrese una temperatura válida")]
        public decimal VitalTemperatura { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de creación es obligatoria")]
        public DateTime CreadoEn { get; set; } 
    }
}
