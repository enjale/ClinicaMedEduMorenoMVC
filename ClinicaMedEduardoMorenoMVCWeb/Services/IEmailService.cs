namespace ClinicaMedEduardoMorenoMVCWeb.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
        Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink);
    }
}
