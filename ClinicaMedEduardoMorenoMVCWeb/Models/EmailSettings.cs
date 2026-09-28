namespace ClinicaMedEduardoMorenoMVCWeb.Models
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string SenderName { get; set; } = "Clínica Médica Eduardo Moreno";
        public string SenderEmail { get; set; } = string.Empty;
        public string AppPassword { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
    }
}
