using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvitacionesAPI.Data;
using InvitacionesAPI.Models;
using InvitacionesAPI.DTOs;
using InvitacionesAPI.Services;

namespace InvitacionesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfirmationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<ConfirmationsController> _logger;

        public ConfirmationsController(
            ApplicationDbContext context,
            IEmailService emailService,
            ILogger<ConfirmationsController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        // POST: api/confirmations/{invitationId}
        [HttpPost("{invitationId}")]
        public async Task<ActionResult> CreateConfirmation(Guid invitationId, ConfirmationDto dto)
        {
            var invitation = await _context.Invitations.FindAsync(invitationId);

            if (invitation == null)
            {
                return NotFound("Invitation not found");
            }

            var confirmation = new Confirmation
            {
                InvitationId = invitationId,
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                WillAttend = dto.WillAttend,
                NumberOfGuests = dto.NumberOfGuests,
                Message = dto.Message,
                CreatedAt = DateTime.UtcNow
            };

            _context.Confirmations.Add(confirmation);
            await _context.SaveChangesAsync();

            // Send email notification to the invitation owner
            if (!string.IsNullOrEmpty(invitation.FormEmail))
            {
                try
                {
                    await _emailService.SendConfirmationEmailAsync(
                        invitation.FormEmail,
                        dto.Name,
                        invitation.Title,
                        dto.WillAttend,
                        dto.NumberOfGuests
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending confirmation email");
                    // Continue even if email fails
                }
            }

            return Ok(new { message = "Confirmation submitted successfully" });
        }

        // GET: api/confirmations/invitation/{invitationId}
        [HttpGet("invitation/{invitationId}")]
        public async Task<ActionResult<IEnumerable<Confirmation>>> GetConfirmationsByInvitation(Guid invitationId)
        {
            var confirmations = await _context.Confirmations
                .Where(c => c.InvitationId == invitationId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Ok(confirmations);
        }

        // POST: api/confirmations/send-email
        [HttpPost("send-email")]
        public async Task<IActionResult> SendConfirmationEmail([FromBody] FormConfirmationRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.ToEmail))
                {
                    return BadRequest(new { error = "Email de destino es requerido" });
                }

                if (string.IsNullOrEmpty(request.Phone))
                {
                    return BadRequest(new { error = "Teléfono es requerido" });
                }

                if (request.FormData == null || request.FormData.Count == 0)
                {
                    return BadRequest(new { error = "Datos del formulario son requeridos" });
                }

                // DEBUG: Log received data
                _logger.LogInformation("=== DEBUG RECEIVED DATA ===");
                _logger.LogInformation($"ToEmail: {request.ToEmail}");
                _logger.LogInformation($"Phone: {request.Phone}");
                _logger.LogInformation($"FormData count: {request.FormData.Count}");
                foreach (var kvp in request.FormData)
                {
                    _logger.LogInformation($"  Key: '{kvp.Key}' => Value: '{kvp.Value}'");
                }
                _logger.LogInformation("==========================");

                try
                {
                    await _emailService.SendFormConfirmationEmailAsync(
                        request.ToEmail,
                        request.Phone,
                        request.FormData
                    );

                    _logger.LogInformation($"Confirmation email sent to {request.ToEmail} for phone {request.Phone}");

                    return Ok(new { success = true, message = "Email enviado correctamente" });
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Error sending email, but confirmation was saved");
                    // Return success even if email fails - the confirmation was saved
                    return Ok(new { success = true, message = "Confirmación guardada (email no configurado)", warning = "El email no pudo ser enviado pero la confirmación fue guardada." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing confirmation");
                return StatusCode(500, new { error = "Error al procesar la confirmación", details = ex.Message });
            }
        }
    }

    public class FormConfirmationRequest
    {
        public string ToEmail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public Dictionary<string, string> FormData { get; set; } = new Dictionary<string, string>();
    }
}
