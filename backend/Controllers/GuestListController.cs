using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvitacionesAPI.Data;
using InvitacionesAPI.Models;
using InvitacionesAPI.Services;

namespace InvitacionesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestListController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ExcelParserService _excelParser;

        public GuestListController(ApplicationDbContext context, ExcelParserService excelParser)
        {
            _context = context;
            _excelParser = excelParser;
        }

        // POST: api/GuestList/upload/{invitationId}
        [HttpPost("upload/{invitationId}")]
        public async Task<ActionResult<object>> UploadExcel(Guid invitationId, IFormFile file)
        {
            Console.WriteLine($"=== UPLOAD EXCEL DEBUG ===");
            Console.WriteLine($"Received invitation ID: {invitationId}");
            Console.WriteLine($"File received: {file?.FileName}");

            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No se proporcionó archivo" });
            }

            // Validate file extension
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
            {
                return BadRequest(new { message = "El archivo debe ser un Excel (.xlsx o .xls)" });
            }

            // Check if invitation exists
            Console.WriteLine($"Searching for invitation with ID: {invitationId}");
            var invitation = await _context.Invitations.FindAsync(invitationId);
            Console.WriteLine($"Invitation found: {invitation != null}");

            if (invitation == null)
            {
                // List all invitations in database for debugging
                var allInvitations = await _context.Invitations.Select(i => i.Id).ToListAsync();
                Console.WriteLine($"All invitation IDs in database: {string.Join(", ", allInvitations)}");
                return NotFound(new { message = "Invitación no encontrada" });
            }

            try
            {
                // Parse Excel file
                using var stream = file.OpenReadStream();
                var guests = await _excelParser.ParseExcelFile(stream, invitationId);

                // Remove existing guests for this invitation
                var existingGuests = await _context.GuestList
                    .Where(g => g.InvitationId == invitationId)
                    .ToListAsync();
                _context.GuestList.RemoveRange(existingGuests);

                // Add new guests
                await _context.GuestList.AddRangeAsync(guests);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Lista de invitados cargada exitosamente",
                    totalGuests = guests.Count,
                    guests = guests
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine($"Error uploading Excel: {ex.Message}");
                Console.WriteLine($"Inner exception: {innerMessage}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { message = $"Error al procesar el archivo: {innerMessage}" });
            }
        }

        // GET: api/GuestList/{invitationId}
        [HttpGet("{invitationId}")]
        public async Task<ActionResult<object>> GetGuestsByInvitation(
            Guid invitationId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var totalGuests = await _context.GuestList
                .Where(g => g.InvitationId == invitationId)
                .CountAsync();

            var guests = await _context.GuestList
                .Where(g => g.InvitationId == invitationId)
                .OrderBy(g => g.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                totalGuests,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(totalGuests / (double)pageSize),
                guests
            });
        }

        // GET: api/GuestList/{invitationId}/phone/{phone}
        [HttpGet("{invitationId}/phone/{phone}")]
        public async Task<ActionResult<GuestListItem>> GetGuestByPhone(Guid invitationId, string phone)
        {
            var guest = await _context.GuestList
                .FirstOrDefaultAsync(g => g.InvitationId == invitationId && g.Phone == phone);

            if (guest == null)
            {
                return NotFound(new { message = "Invitado no encontrado" });
            }

            return Ok(guest);
        }

        // PUT: api/GuestList/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGuest(int id, GuestListItem guest)
        {
            if (id != guest.Id)
            {
                return BadRequest(new { message = "ID no coincide" });
            }

            var existingGuest = await _context.GuestList.FindAsync(id);
            if (existingGuest == null)
            {
                return NotFound(new { message = "Invitado no encontrado" });
            }

            // Update properties
            existingGuest.Name = guest.Name;
            existingGuest.Phone = guest.Phone;
            existingGuest.NumberOfGuests = guest.NumberOfGuests;
            existingGuest.GuestNames = guest.GuestNames;
            existingGuest.EventHasAccommodation = guest.EventHasAccommodation;
            existingGuest.AccommodationAllowed = guest.AccommodationAllowed;
            existingGuest.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Error al actualizar" });
            }

            return Ok(new { message = "Invitado actualizado exitosamente", guest = existingGuest });
        }

        // DELETE: api/GuestList/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuest(int id)
        {
            var guest = await _context.GuestList.FindAsync(id);
            if (guest == null)
            {
                return NotFound(new { message = "Invitado no encontrado" });
            }

            _context.GuestList.Remove(guest);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Invitado eliminado exitosamente" });
        }

        // POST: api/GuestList/{invitationId}/guest
        [HttpPost("{invitationId}/guest")]
        public async Task<ActionResult<GuestListItem>> AddGuest(Guid invitationId, GuestListItem guest)
        {
            // Check if invitation exists
            var invitation = await _context.Invitations.FindAsync(invitationId);
            if (invitation == null)
            {
                return NotFound(new { message = "Invitación no encontrada" });
            }

            // Check if phone already exists for this invitation
            var existingGuest = await _context.GuestList
                .FirstOrDefaultAsync(g => g.InvitationId == invitationId && g.Phone == guest.Phone);

            if (existingGuest != null)
            {
                return BadRequest(new { message = "Ya existe un invitado con ese número de teléfono" });
            }

            guest.InvitationId = invitationId;
            guest.CreatedAt = DateTime.UtcNow;

            _context.GuestList.Add(guest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGuestByPhone),
                new { invitationId = guest.InvitationId, phone = guest.Phone },
                guest);
        }

        // DELETE: api/GuestList/{invitationId}/clear
        [HttpDelete("{invitationId}/clear")]
        public async Task<IActionResult> ClearGuestList(Guid invitationId)
        {
            var guests = await _context.GuestList
                .Where(g => g.InvitationId == invitationId)
                .ToListAsync();

            _context.GuestList.RemoveRange(guests);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Se eliminaron {guests.Count} invitados" });
        }
    }
}
