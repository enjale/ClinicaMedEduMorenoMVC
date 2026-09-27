using Microsoft.AspNetCore.Mvc;
using ClinicaMedEduardoMorenoMVCWeb.Data;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class ExpedienteAlergias
    {
        [Key]
        public int ExpedienteAlergiaId { get; set; }

        [Required(ErrorMessage = "El expediente es obligatorio")]
        public int ExpedienteId { get; set; }

        [Required(ErrorMessage = "La alergia es obligatoria")]
        public int AlergiaId { get; set; }

        [Required(ErrorMessage = "El nivel es obligatorio")]
        [StringLength(100)]
        public string Nivel { get; set; } = string.Empty;

        [StringLength(500)]
        public string Observaciones { get; set; } = string.Empty;
    }
}