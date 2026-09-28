using System;
using System.Collections.Generic;
using ClinicaMedEduardoMorenoMVCWeb.Models;

namespace ClinicaMedEduardoMorenoMVCWeb.Models.ViewModels
{
    // Vista de solo lectura del expediente clínico completo de un paciente
    public class ExpedienteCompletoViewModel
    {
        public int ExpedienteId { get; set; }
        public string CodigoExpediente { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public string? Historial { get; set; }

        // Consulta desde la que se abrió el expediente (para el botón "Volver")
        public int? ConsultaOrigenId { get; set; }

        public Pacientes Paciente { get; set; } = new();
        public int? Edad { get; set; }

        public List<ExpedienteAlergiaItem> Alergias { get; set; } = new();
        public List<ExpedienteEnfermedadItem> Enfermedades { get; set; } = new();
        public List<AntecedenteItem> Antecedentes { get; set; } = new();
        public List<HistorialConsultaItem> Consultas { get; set; } = new();
    }

    // Datos leídos del expediente con SQL (las columnas pueden venir en NULL)
    public class ExpedienteDatos
    {
        public string Codigo { get; set; } = string.Empty;
        public int PacienteId { get; set; }
        public string? Historial { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class ExpedienteAlergiaItem
    {
        public string Alergia { get; set; } = string.Empty;
        public string Nivel { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }

    public class ExpedienteEnfermedadItem
    {
        public string Enfermedad { get; set; } = string.Empty;
        public string TipoEnfermedad { get; set; } = string.Empty;
        public DateTime? FechaDeteccion { get; set; }
        public string? Observaciones { get; set; }
    }

    public class AntecedenteItem
    {
        public string Tipo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string? Observaciones { get; set; }
    }

    public class HistorialConsultaItem
    {
        public int ConsultaId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string? Medico { get; set; }
        public List<string> Diagnosticos { get; set; } = new();
        public List<string> Medicamentos { get; set; } = new();
    }
}
