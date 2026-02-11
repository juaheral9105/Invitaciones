using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SendGrid;
using SendGrid.Helpers.Mail;

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
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUser = _configuration["Email:SmtpUser"];
                var smtpPassword = _configuration["Email:SmtpPassword"];
                var fromEmail = _configuration["Email:FromEmail"];
                var fromName = _configuration["Email:FromName"] ?? "Invitaciones XV Años";

                if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPassword))
                {
                    _logger.LogWarning("Email configuration is missing. Skipping email send.");
                    return;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail ?? smtpUser));
                message.To.Add(new MailboxAddress(guestName, toEmail));
                message.Subject = $"Confirmación de asistencia - {eventName}";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif; padding: 20px;'>
                            <h2 style='color: #8B5CF6;'>Nueva confirmación de asistencia</h2>
                            <div style='background-color: #f3f4f6; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                                <p><strong>Invitado:</strong> {guestName}</p>
                                <p><strong>Asistencia:</strong> {(willAttend ? "✅ Sí asistirá" : "❌ No asistirá")}</p>
                                <p><strong>Número de invitados:</strong> {numberOfGuests}</p>
                                <p><strong>Evento:</strong> {eventName}</p>
                            </div>
                            <p style='color: #6b7280; font-size: 14px;'>Este es un mensaje automático del sistema de invitaciones.</p>
                        </body>
                        </html>
                    "
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                // Set short timeout (3 seconds) for fast failure when SMTP not configured
                client.Timeout = 60000;

                client.LocalDomain = "localhost";
                _logger.LogInformation("Connecting to SMTP...");
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                _logger.LogInformation("Connected!");

                _logger.LogInformation("Authenticating...");
                await client.AuthenticateAsync(smtpUser, smtpPassword);
                _logger.LogInformation("Authenticated!");
                //await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                //await client.AuthenticateAsync(smtpUser, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Email not sent (SMTP not configured or failed). Confirmation still processed successfully. Error: {ex.Message}");
                // Don't throw exception - confirmation is still successful even without email
            }
        }

        public async Task SendFormConfirmationEmailAsync(string toEmail, string phone, Dictionary<string, string> formData)
        {
            try
            {
                // Debug: Log ALL environment variables that start with "Email"
                _logger.LogInformation("=== EMAIL CONFIG DEBUG ===");
                _logger.LogInformation("ALL Environment Variables starting with 'Email':");
                foreach (var envVar in Environment.GetEnvironmentVariables().Keys)
                {
                    var key = envVar.ToString();
                    if (key?.StartsWith("Email", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        var value = Environment.GetEnvironmentVariable(key);
                        // Mask password if present
                        var displayValue = key.Contains("Password", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(value)
                            ? "***MASKED***"
                            : value ?? "NOT SET";
                        _logger.LogInformation("  {Key} = {Value}", key, displayValue);
                    }
                }

                _logger.LogInformation("Direct checks:");
                _logger.LogInformation("  Email__SmtpHost: {EnvHost}", Environment.GetEnvironmentVariable("Email__SmtpHost") ?? "NOT SET");
                _logger.LogInformation("  Email__SmtpUser: {EnvUser}", Environment.GetEnvironmentVariable("Email__SmtpUser") ?? "NOT SET");

                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUser = _configuration["Email:SmtpUser"];
                var smtpPassword = _configuration["Email:SmtpPassword"];
                var fromEmail = _configuration["Email:FromEmail"];
                var fromName = _configuration["Email:FromName"] ?? "Sistema de Invitaciones";

                _logger.LogInformation("Final SMTP Host: {Host}", smtpHost);
                _logger.LogInformation("Final SMTP Port: {Port}", smtpPort);
                _logger.LogInformation("SMTP User configured: {Configured}", !string.IsNullOrEmpty(smtpUser) ? "YES" : "NO");
                _logger.LogInformation("========================");

                if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPassword))
                {
                    _logger.LogWarning("Email configuration is missing. Skipping email send.");
                    return;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail ?? smtpUser));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = "Confirmación de Invitación";

                // Build plain text body with form data
                var bodyText = "=== CONFIRMACIÓN DE INVITACIÓN ===\n\n";

                // Extract and display name and guests prominently
                string guestName = "";
                string guestNames = "";

                // DEBUG: Log all form data keys
                _logger.LogInformation("=== EmailService DEBUG ===");
                _logger.LogInformation($"Total form fields: {formData.Count}");
                foreach (var key in formData.Keys)
                {
                    _logger.LogInformation($"  Field: '{key}' = '{formData[key]}'");
                }

                // Look for guest name in different possible field names
                foreach (var key in formData.Keys)
                {
                    var lowerKey = key.ToLower();
                    _logger.LogInformation($"Checking key: '{key}' (lower: '{lowerKey}')");

                    // Look for main guest name (nombre, but not acompañante)
                    if (string.IsNullOrEmpty(guestName) &&
                        lowerKey.Contains("nombre") &&
                        !lowerKey.Contains("acompañante") &&
                        !lowerKey.Contains("invitados"))
                    {
                        guestName = formData[key];
                        _logger.LogInformation($"✓ FOUND guestName: '{guestName}' from key '{key}'");
                    }

                    // Look for guest names/companions
                    if (string.IsNullOrEmpty(guestNames) &&
                        ((lowerKey.Contains("acompañante") && lowerKey.Contains("nombre")) ||
                         lowerKey.Contains("nombres de acompañantes") ||
                         (lowerKey.Contains("invitado") && lowerKey.Contains("nombre") && !lowerKey.Equals("nombre del invitado", StringComparison.OrdinalIgnoreCase))))
                    {
                        guestNames = formData[key];
                        _logger.LogInformation($"✓ FOUND guestNames: '{guestNames}' from key '{key}'");
                    }
                }

                _logger.LogInformation($"Final guestName: '{guestName}'");
                _logger.LogInformation($"Final guestNames: '{guestNames}'");
                _logger.LogInformation("========================");

                // Display name and guests at the top
                if (!string.IsNullOrEmpty(guestName))
                {
                    bodyText += $"👤 INVITADO PRINCIPAL: {guestName}\n";
                }

                if (!string.IsNullOrEmpty(guestNames))
                {
                    bodyText += $"👥 ACOMPAÑANTES: {guestNames}\n";
                }

                if (!string.IsNullOrEmpty(guestName) || !string.IsNullOrEmpty(guestNames))
                {
                    bodyText += "\n";
                }

                bodyText += $"📱 TELÉFONO: {phone}\n\n";
                bodyText += "DATOS DEL FORMULARIO:\n";
                bodyText += "------------------------\n\n";

                foreach (var field in formData)
                {
                    bodyText += $"{field.Key}: {field.Value}\n";
                }

                bodyText += "\n------------------------\n";
                bodyText += "Este es un mensaje automático del sistema de invitaciones.";

                var bodyBuilder = new BodyBuilder
                {
                    TextBody = bodyText
                };

                message.Body = bodyBuilder.ToMessageBody();
                
               // using var client = new SmtpClient();
                // Set short timeout (3 seconds) for fast failure when SMTP not configured
              // client.AuthenticationMechanisms.Remove("XOAUTH2");
              // client.Timeout = 60000;

              //  client.LocalDomain = "localhost";
              //  _logger.LogInformation("Connecting to SMTP...");
              //  await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
              //  _logger.LogInformation("Connected!");

              //  _logger.LogInformation("Authenticating...");
              //  await client.AuthenticateAsync(smtpUser, smtpPassword);
              //  _logger.LogInformation("Authenticated!");
              //  await client.SendAsync(message);
              //  await client.DisconnectAsync(true);


             //string plainTextContent = bodyBuilder.ToMessageBody();

            var client = new SendGridClient(Environment.GetEnvironmentVariable("SENDGRID_API_KEY"));

            var from = new EmailAddress(
                Environment.GetEnvironmentVariable("EMAIL_FROM"),
                Environment.GetEnvironmentVariable("EMAIL_FROM_NAME")
            );

            var to = new EmailAddress("nuryvanessamesa@gmail.com");
            var msg = MailHelper.CreateSingleEmail(
                from,
                to,
                "Prueba SendGrid",
                "Correo de prueba",
                "<strong>Correo de prueba</strong>"
            );

            var response = await client.SendEmailAsync(msg);

            _logger.LogInformation("SendGrid StatusCode: {StatusCode}", response.StatusCode);

            var body = await response.Body.ReadAsStringAsync();
            _logger.LogInformation("SendGrid Response Body: {Body}", body);

            _logger.LogInformation($"Form confirmation email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Email not sent (SMTP not configured or failed). Confirmation still processed successfully. Error: {ex.Message}");
                // Don't throw exception - confirmation is still successful even without email
            }
        }
    }
}
