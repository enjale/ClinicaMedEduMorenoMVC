using Microsoft.AspNetCore.Mvc;
using ClinicaMedEduardoMorenoMVCWeb.Data;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class ConsultaEnfermedades
    {
        [Key]
        public int ConsultaEnfermedadId { get; set; }

        [Required(ErrorMessage = "La consulta es obligatoria")]
        public int ConsultaId { get; set; }

        [Required(ErrorMessage = "La enfermedad es obligatoria")]
        public int EnfermedadId { get; set; }

        [StringLength(500)]
        public string Diagnostico { get; set; } = string.Empty;
    }
}
