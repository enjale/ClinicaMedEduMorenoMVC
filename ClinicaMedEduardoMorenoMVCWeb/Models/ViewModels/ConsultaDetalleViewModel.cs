using System.Collections.Generic;
using ClinicaMedEduardoMorenoMVCWeb.Models;

namespace ClinicaMedEduardoMorenoMVCWeb.Models.ViewModels
{
    public class ConsultaDetalleViewModel
    {
        // Datos de la consulta
        public Consultas Consulta { get; set; } = new();

        // Datos del paciente y del usuario que registró
        public string NombrePaciente { get; set; } = string.Empty;
        public string CodigoPaciente { get; set; } = string.Empty;
        public string CodigoExpediente { get; set; } = string.Empty;
        public int? EdadPaciente { get; set; }
        public string SexoPaciente { get; set; } = string.Empty;
        public string RegistradoPor { get; set; } = string.Empty;

        // Padecimientos y diagnósticos asignados a la consulta
        public List<ConsultaEnfermedadItem> Enfermedades { get; set; } = new();

        // Receta de la consulta (null si aún no se ha recetado nada) y sus medicamentos
        public Recetas? Receta { get; set; }
        public List<RecetaMedicamentoItem> Medicamentos { get; set; } = new();
    }

    public class RecetaMedicamentoItem
    {
        public int RecetaDetalleId { get; set; }
        public byte Orden { get; set; }
        public string CodigoMedicamento { get; set; } = string.Empty;
        public string NombreMedicamento { get; set; } = string.Empty;
        public string Prescripcion { get; set; } = string.Empty;
        public string Cantidad { get; set; } = string.Empty;
    }

    public class ConsultaEnfermedadItem
    {
        public int ConsultaEnfermedadId { get; set; }
        public string CodigoEnfermedad { get; set; } = string.Empty;
        public string NombreEnfermedad { get; set; } = string.Empty;
        public string TipoEnfermedad { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }
}
