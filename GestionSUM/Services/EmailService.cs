using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration; // Necesario para leer appsettings

namespace GestionSUM.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        // Inyectamos IConfiguration para leer EmailSettings
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarConfirmacionReservaAsync(string emailDestino, string nombreUsuario, string fecha, string turno)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Gestión SUM", "agustin.hcarabajal@gmail.com"));
            email.To.Add(new MailboxAddress(nombreUsuario, emailDestino));
            email.Subject = "Confirmación de Reserva - SUM";

            email.Body = new TextPart("html")
            {
                Text = $@"
                    <div style='font-family: sans-serif; border: 1px solid #ddd; padding: 20px; border-radius: 10px;'>
                        <h2 style='color: #28a745;'>¡Hola {nombreUsuario}!</h2>
                        <p>Tu reserva para el SUM ha sido confirmada.</p>
                        <hr>
                        <p><strong>Fecha:</strong> {fecha}</p>
                        <p><strong>Turno:</strong> {turno}</p>
                        <hr>
                        <p>Si desea cancelar la reserva comunicarse con el admnistrador.</p>
                        <hr>
                        <p style='font-size: 0.8em; color: #666;'>Gracias por usar GestionSUM.</p>
                    </div>"
            };

            using var smtp = new SmtpClient();
            try
            {
                // Conexión a Brevo
                await smtp.ConnectAsync("smtp-relay.brevo.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                // Leemos las credenciales desde el appsettings.json
                var user = _configuration["EmailSettings:SmtpUser"];
                var pass = _configuration["EmailSettings:SmtpPass"];

                await smtp.AuthenticateAsync(user, pass);
                await smtp.SendAsync(email);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}