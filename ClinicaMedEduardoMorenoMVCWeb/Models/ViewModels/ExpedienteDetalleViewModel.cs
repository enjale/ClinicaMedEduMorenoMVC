using ClinicaMedEduardoMorenoMVCWeb.Models;

namespace ClinicaMedEduardoMorenoMVCWeb.Models.ViewModels
{
    public class ExpedienteDetalleViewModel
    {
        public Expedientes Expediente { get; set; } = new();
        public Pacientes? Paciente { get; set; }
        public List<ExpedienteAlergias> Alergias { get; set; } = new();
        public List<ExpedienteEnfermedades> Enfermedades { get; set; } = new();
        public List<Antecedentes> Antecedentes { get; set; } = new();
        public List<Consultas> Consultas { get; set; } = new();
        public string TabActiva { get; set; } = "alergias";
    }
}
