using System;
using System.Collections.Generic;

namespace ClinicaMedEduardoMorenoMVCWeb.Models.ViewModels
{
    public class EnfermeraDashboardViewModel
    {
        // Información de la Enfermera
        public string NombreEnfermera { get; set; } = string.Empty;
        public DateTime FechaHoy { get; set; } = DateTime.Today;
    }
}
