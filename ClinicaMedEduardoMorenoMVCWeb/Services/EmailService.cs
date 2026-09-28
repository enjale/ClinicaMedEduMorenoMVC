using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using ClinicaMedEduardoMorenoMVCWeb.Models;

namespace ClinicaMedEduardoMorenoMVCWeb.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlMessage
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                client.CheckCertificateRevocation = false;
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                // Conectar usando StartTls o Auto
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                
                var appPassword = _emailSettings.AppPassword?.Replace(" ", "").Trim() ?? string.Empty;
                var senderEmail = _emailSettings.SenderEmail?.Trim() ?? string.Empty;

                await client.AuthenticateAsync(senderEmail, appPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Correo enviado exitosamente a {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo electrónico a {Email}", toEmail);
                throw;
            }
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink)
        {
            string subject = "Recuperación de Contraseña - Clínica Médica Eduardo Moreno";
            
            string htmlContent = $@"
            <div style=""font-family: 'Segoe UI', Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 25px; border: 1px solid #D6DDD8; border-radius: 8px; background-color: #FAFAF9;"">
                <div style=""text-align: center; padding-bottom: 20px; border-bottom: 2px solid #4F6F52;"">
                    <h3 style=""color: #A3B18A; margin: 0; font-size: 14px; letter-spacing: 2px; text-transform: uppercase;"">Clínica Médica</h3>
                    <h1 style=""color: #4F6F52; margin: 5px 0 0 0; font-size: 24px;"">Eduardo Moreno</h1>
                </div>
                
                <div style=""padding: 25px 0;"">
                    <p style=""font-size: 16px; color: #374151;"">Estimado(a) <strong>{userName}</strong>,</p>
                    <p style=""font-size: 15px; color: #374151; line-height: 1.5;"">
                        Hemos recibido una solicitud para restablecer la contraseña de su cuenta en el Sistema de Gestión de Expedientes Médicos.
                    </p>
                    <p style=""font-size: 15px; color: #374151; line-height: 1.5;"">
                        Para continuar con el restablecimiento, por favor haga clic en el siguiente botón:
                    </p>
                    
                    <div style=""text-align: center; margin: 30px 0;"">
                        <a href=""{resetLink}"" style=""background-color: #4F6F52; color: #ffffff; padding: 12px 28px; text-decoration: none; border-radius: 6px; font-weight: bold; display: inline-block; font-size: 15px;"">
                            Restablecer mi Contraseña
                        </a>
                    </div>
                    
                    <p style=""font-size: 13px; color: #6B7280; line-height: 1.4;"">
                        Si el botón anterior no funciona, puede copiar y pegar el siguiente enlace en su navegador:<br/>
                        <a href=""{resetLink}"" style=""color: #6B8E7A; word-break: break-all;"">{resetLink}</a>
                    </p>
                    
                    <div style=""background-color: #FAEFDA; border-left: 4px solid #633806; padding: 12px; margin-top: 25px; border-radius: 4px;"">
                        <p style=""margin: 0; font-size: 13px; color: #633806;"">
                            <strong>Importante:</strong> Este enlace tiene una vigencia limitada por seguridad. Si usted no solicitó este cambio, puede ignorar este mensaje y su contraseña actual permanecerá segura.
                        </p>
                    </div>
                </div>
                
                <div style=""text-align: center; padding-top: 20px; border-top: 1px solid #D6DDD8; font-size: 12px; color: #9CA3AF;"">
                    <p style=""margin: 0;"">© {DateTime.Now.Year} Clínica Médica Eduardo Moreno. Todos los derechos reservados.</p>
                </div>
            </div>";

            await SendEmailAsync(toEmail, subject, htmlContent);
        }
    }
}
