using Microsoft.AspNetCore.Mvc;
using ClinicaMedEduardoMorenoMVCWeb.Data;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class Consultas
    {
        [Key]
        public int ConsultaId { get; set; }

        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(20)]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El expediente es obligatorio")]
        [Display(Name = "Paciente / Expediente")]
        public int ExpedienteId { get; set; }

        [Required(ErrorMessage = "El usuario que registra es obligatorio")]
        [Display(Name = "Registrado por")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [StringLength(500)]
        [Display(Name = "Motivo de consulta")]
        public string Motivo { get; set; } = string.Empty;

        [StringLength(150)]
        [Display(Name = "Médico")]
        public string? Medico { get; set; }

        [Range(0.1, 500, ErrorMessage = "Ingrese un peso válido")]
        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Peso (kg)")]
        public decimal? VitalPeso { get; set; }

        [Range(30, 45, ErrorMessage = "Ingrese una temperatura válida")]
        [Column(TypeName = "decimal(4,1)")]
        [Display(Name = "Temperatura (°C)")]
        public decimal? VitalTemperatura { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^\d{2,3}/\d{2,3}$", ErrorMessage = "Formato de presión no válido (ej. 120/80)")]
        [Display(Name = "Presión arterial")]
        public string? VitalPresion { get; set; }

        [StringLength(1000)]
        [Display(Name = "Diagnóstico")]
        public string? Diagnostico { get; set; }

        [StringLength(1000)]
        [Display(Name = "Tratamiento")]
        public string? Tratamiento { get; set; }

        [StringLength(1000)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(20)]
        public string Estado { get; set; } = "pendiente";

        [Display(Name = "Creado en")]
        public DateTime CreadoEn { get; set; }
    }
}
