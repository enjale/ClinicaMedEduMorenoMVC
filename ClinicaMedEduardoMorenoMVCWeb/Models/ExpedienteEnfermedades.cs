using Microsoft.AspNetCore.Mvc;
using ClinicaMedEduardoMorenoMVCWeb.Data;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class ExpedienteEnfermedades
    {
        [Key]
        public int ExpedienteEnfermedadId { get; set; }

        [Required(ErrorMessage = "El expediente es obligatorio")]
        public int ExpedienteId { get; set; }

        [Required(ErrorMessage = "La enfermedad es obligatoria")]
        public int EnfermedadId { get; set; }

        [Required(ErrorMessage = "La fecha de detección es obligatoria")]
        public DateTime FechaDeteccion { get; set; }

        [StringLength(500)]
        public string Observaciones { get; set; } = string.Empty;
    }
}