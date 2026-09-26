using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class ContactoEmergencia
    {
        public int ContactoEmergenciaId { get; set; }
        public int PacienteId { get; set; }

        [Required(ErrorMessage= "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La relacion es obligatoria")]
        public string Relacion {  get; set; } = string.Empty;

        [Required(ErrorMessage = "El telefono es obligatorio")]
        public string Telefono {  get; set; } = string.Empty;

        [Required(ErrorMessage = "El telefono alterno obligatorio")]
        public string TelefonoAlterno {  get; set; } = string.Empty;

        [Required(ErrorMessage = "La direccion es obligatoria")]
        public string Direccion {  get; set; } = string.Empty;

        [Required(ErrorMessage = "La prioridad es obligatoria")]
        public int Prioridad { get; set; }
        public bool Activo {get ; set; }
        [Required(ErrorMessage = "La fecha de creacion es obligatoria")]
        public DateTime CreadoEn {  get; set; }

    }
}
