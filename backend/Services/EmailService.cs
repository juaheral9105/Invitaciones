using Resend;

namespace InvitacionesAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendConfirmationEmailAsync(string toEmail, string guestName, string eventName, bool willAttend, int numberOfGuests)
        {
            try
            {
                var apiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY");
                var fromEmail = Environment.GetEnvironmentVariable("RESEND_FROM_EMAIL") ?? "onboarding@resend.dev";

                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogWarning("Resend API key is missing. Skipping email send.");
                    return;
                }

                var client = new ResendClient(apiKey);

                var htmlBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2 style='color: #8B5CF6;'>Nueva confirmacion de asistencia</h2>
                        <div style='background-color: #f3f4f6; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                            <p><strong>Invitado:</strong> {guestName}</p>
                            <p><strong>Asistencia:</strong> {(willAttend ? "Si asistira" : "No asistira")}</p>
                            <p><strong>Numero de invitados:</strong> {numberOfGuests}</p>
                            <p><strong>Evento:</strong> {eventName}</p>
                        </div>
                        <p style='color: #6b7280; font-size: 14px;'>Este es un mensaje automatico del sistema de invitaciones.</p>
                    </body>
                    </html>
                ";

                var message = new EmailMessage
                {
                    From = fromEmail,
                    Subject = $"Confirmacion de asistencia - {eventName}",
                    HtmlBody = htmlBody
                };
                message.To.Add(toEmail);

                var response = await client.EmailSendAsync(message);

                _logger.LogInformation("Resend response - Email sent with ID: {Id}", response?.Id);
                _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Email not sent. Error: {Error}", ex.Message);
            }
        }

        public async Task SendFormConfirmationEmailAsync(string toEmail, string phone, Dictionary<string, string> formData)
        {
            try
            {
                var apiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY");
                var fromEmail = Environment.GetEnvironmentVariable("RESEND_FROM_EMAIL") ?? "onboarding@resend.dev";

                _logger.LogInformation("=== RESEND CONFIG DEBUG ===");
                _logger.LogInformation("API Key configured: {Configured}", !string.IsNullOrEmpty(apiKey) ? "YES" : "NO");
                _logger.LogInformation("From Email: {From}", fromEmail);
                _logger.LogInformation("To Email: {To}", toEmail);
                _logger.LogInformation("===========================");

                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogWarning("Resend API key is missing. Skipping email send.");
                    return;
                }

                // Extract guest name from form data
                string guestName = "";
                string guestNames = "";

                _logger.LogInformation("=== Form Data DEBUG ===");
                _logger.LogInformation("Total form fields: {Count}", formData.Count);

                foreach (var key in formData.Keys)
                {
                    _logger.LogInformation("  Field: '{Key}' = '{Value}'", key, formData[key]);

                    var lowerKey = key.ToLower();

                    if (string.IsNullOrEmpty(guestName) &&
                        lowerKey.Contains("nombre") &&
                        !lowerKey.Contains("acompanante") &&
                        !lowerKey.Contains("invitados"))
                    {
                        guestName = formData[key];
                        _logger.LogInformation("Found guestName: '{Name}' from key '{Key}'", guestName, key);
                    }

                    if (string.IsNullOrEmpty(guestNames) &&
                        ((lowerKey.Contains("acompanante") && lowerKey.Contains("nombre")) ||
                         lowerKey.Contains("nombres de acompanantes") ||
                         (lowerKey.Contains("invitado") && lowerKey.Contains("nombre") && !lowerKey.Equals("nombre del invitado", StringComparison.OrdinalIgnoreCase))))
                    {
                        guestNames = formData[key];
                        _logger.LogInformation("Found guestNames: '{Names}' from key '{Key}'", guestNames, key);
                    }
                }
                _logger.LogInformation("=======================");

                // Build HTML email body
                var formFieldsHtml = string.Join("", formData.Select(f => $@"
                    <tr>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb; font-weight: bold; color: #6b7280;'>{f.Key}</td>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'>{f.Value}</td>
                    </tr>
                "));

                var htmlBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; padding: 20px; background-color: #f9fafb;'>
                        <div style='max-width: 600px; margin: 0 auto; background-color: white; border-radius: 12px; padding: 30px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                            <h2 style='color: #8B5CF6; margin-bottom: 20px; border-bottom: 2px solid #8B5CF6; padding-bottom: 10px;'>
                                Confirmacion de Invitacion
                            </h2>

                            {(string.IsNullOrEmpty(guestName) ? "" : $"<p style='font-size: 18px;'><strong>Invitado Principal:</strong> {guestName}</p>")}
                            {(string.IsNullOrEmpty(guestNames) ? "" : $"<p><strong>Acompanantes:</strong> {guestNames}</p>")}
                            <p><strong>Telefono:</strong> {phone}</p>

                            <hr style='border: none; border-top: 1px solid #e5e7eb; margin: 20px 0;' />

                            <h3 style='color: #374151;'>Datos del Formulario:</h3>
                            <table style='width: 100%; border-collapse: collapse;'>
                                {formFieldsHtml}
                            </table>

                            <p style='color: #9ca3af; font-size: 12px; margin-top: 30px; text-align: center;'>
                                Este es un mensaje automatico del sistema de invitaciones.
                            </p>
                        </div>
                    </body>
                    </html>
                ";

                var client = new ResendClient(apiKey);

                var subject = string.IsNullOrEmpty(guestName)
                    ? "Nueva Confirmacion de Invitacion"
                    : $"Respuesta de Invitacion: {guestName}";

                var message = new EmailMessage
                {
                    From = fromEmail,
                    Subject = subject,
                    HtmlBody = htmlBody
                };
                message.To.Add(toEmail);

                _logger.LogInformation("Sending email via Resend...");
                var response = await client.EmailSendAsync(message);

                _logger.LogInformation("Resend response - Email sent with ID: {Id}", response?.Id);
                _logger.LogInformation("Form confirmation email sent successfully to {ToEmail}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Email not sent (Resend failed). Error: {Error}", ex.Message);
                _logger.LogWarning("Stack trace: {Stack}", ex.StackTrace);
            }
        }
    }
}
