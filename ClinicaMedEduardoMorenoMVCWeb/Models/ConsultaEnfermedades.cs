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
        [Display(Name = "Enfermedad")]
        public int EnfermedadId { get; set; }

        [Display(Name = "Diagnóstico / Observaciones")]
        public string? Observaciones { get; set; }
    }
}
