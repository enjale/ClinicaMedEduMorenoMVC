using System;
using System.Collections.Generic;
using ClinicaMedEduardoMorenoMVCWeb.Models;

namespace ClinicaMedEduardoMorenoMVCWeb.Models.ViewModels
{
    public class DoctorDashboardViewModel
    {
        // Información del Médico y Fecha
        public int MedicoId { get; set; }
        public string NombreMedico { get; set; } = string.Empty;
        public string Especialidad { get; set; } = "Medicina General";
        public DateTime FechaHoy { get; set; } = DateTime.Today;

        // Métricas / Tarjetas informativas
        public int TotalConsultasHoy { get; set; }
        public int ConsultasPendientes { get; set; }
        public int ConsultasAtendidas { get; set; }
        public int TotalPacientesRegistrados { get; set; }

        // Consultas programadas para el día de hoy
        public List<ConsultaItemDashboard> ConsultasDeHoy { get; set; } = new();

        // Pacientes recientes
        public List<Pacientes> PacientesRecientes { get; set; } = new();
    }

    public class ConsultaItemDashboard
    {
        public int ConsultaId { get; set; }
        public int ExpedienteId { get; set; }
        public string CodigoConsulta { get; set; } = string.Empty;
        public string NombrePaciente { get; set; } = string.Empty;
        public string CodigoPaciente { get; set; } = string.Empty;
        public DateTime Hora { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
