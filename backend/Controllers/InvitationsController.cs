using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvitacionesAPI.Data;
using InvitacionesAPI.Models;
using InvitacionesAPI.DTOs;
using System.Text.Json;

namespace InvitacionesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvitationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InvitationsController> _logger;

        public InvitationsController(
            ApplicationDbContext context,
            ILogger<InvitationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/invitations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvitationDto>>> GetInvitations()
        {
            var invitations = await _context.Invitations
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            var dtos = invitations.Select(MapToDto).ToList();
            return Ok(dtos);
        }

        // GET: api/invitations/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<InvitationDto>> GetInvitation(Guid id)
        {
            var invitation = await _context.Invitations.FindAsync(id);

            if (invitation == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(invitation));
        }

        // POST: api/invitations
        [HttpPost]
        public async Task<ActionResult<InvitationDto>> CreateInvitation(InvitationDto dto)
        {
            var invitation = new Invitation
            {
                Title = dto.Title,
                Subtitle = dto.Subtitle,
                CelebrantName = dto.CelebrantName,
                EventDate = dto.EventDate,
                EventTime = dto.EventTime,
                Venue = dto.Venue,
                Address = dto.Address,
                LocationLat = dto.LocationLat,
                LocationLng = dto.LocationLng,
                BackgroundColor = dto.BackgroundColor,
                BackgroundImage = dto.BackgroundImage,
                TextColor = dto.TextColor,
                FontFamily = dto.FontFamily,
                MusicUrl = dto.MusicUrl,
                SectionsJson = dto.Sections.HasValue ? dto.Sections.Value.GetRawText() : null,
                FormEmail = dto.FormEmail,
                CoverEnabled = dto.CoverEnabled,
                CoverName = dto.CoverName,
                CoverButtonText = dto.CoverButtonText,
                CoverBackgroundColor = dto.CoverBackgroundColor,
                CoverBackgroundImage = dto.CoverBackgroundImage,
                CoverTextColor = dto.CoverTextColor,
                CoverButtonColor = dto.CoverButtonColor,
                CoverButtonTextColor = dto.CoverButtonTextColor,
                CoverFontFamily = dto.CoverFontFamily,
                CoverNameFontSize = dto.CoverNameFontSize,
                CoverButtonFontSize = dto.CoverButtonFontSize,
                CreatedAt = DateTime.UtcNow
            };

            _context.Invitations.Add(invitation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInvitation), new { id = invitation.Id }, MapToDto(invitation));
        }

        // PUT: api/invitations/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvitation(Guid id, InvitationDto dto)
        {
            var invitation = await _context.Invitations.FindAsync(id);

            if (invitation == null)
            {
                return NotFound();
            }

            invitation.Title = dto.Title;
            invitation.Subtitle = dto.Subtitle;
            invitation.CelebrantName = dto.CelebrantName;
            invitation.EventDate = dto.EventDate;
            invitation.EventTime = dto.EventTime;
            invitation.Venue = dto.Venue;
            invitation.Address = dto.Address;
            invitation.LocationLat = dto.LocationLat;
            invitation.LocationLng = dto.LocationLng;
            invitation.BackgroundColor = dto.BackgroundColor;
            invitation.BackgroundImage = dto.BackgroundImage;
            invitation.TextColor = dto.TextColor;
            invitation.FontFamily = dto.FontFamily;
            invitation.MusicUrl = dto.MusicUrl;
            invitation.SectionsJson = dto.Sections.HasValue ? dto.Sections.Value.GetRawText() : null;
            invitation.FormEmail = dto.FormEmail;
            invitation.CoverEnabled = dto.CoverEnabled;
            invitation.CoverName = dto.CoverName;
            invitation.CoverButtonText = dto.CoverButtonText;
            invitation.CoverBackgroundColor = dto.CoverBackgroundColor;
            invitation.CoverBackgroundImage = dto.CoverBackgroundImage;
            invitation.CoverTextColor = dto.CoverTextColor;
            invitation.CoverButtonColor = dto.CoverButtonColor;
            invitation.CoverButtonTextColor = dto.CoverButtonTextColor;
            invitation.CoverFontFamily = dto.CoverFontFamily;
            invitation.CoverNameFontSize = dto.CoverNameFontSize;
            invitation.CoverButtonFontSize = dto.CoverButtonFontSize;
            invitation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/invitations/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvitation(Guid id)
        {
            var invitation = await _context.Invitations.FindAsync(id);

            if (invitation == null)
            {
                return NotFound();
            }

            _context.Invitations.Remove(invitation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/invitations/upload-image
        [HttpPost("upload-image")]
        public async Task<ActionResult<string>> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("Invalid file type");
            }

            try
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                var storedFile = new StoredFile
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Data = memoryStream.ToArray(),
                    Size = file.Length,
                    UploadedAt = DateTime.UtcNow,
                    FileType = "image"
                };

                _context.StoredFiles.Add(storedFile);
                await _context.SaveChangesAsync();

                var fileUrl = $"/api/invitations/files/{storedFile.Id}";
                return Ok(new { url = fileUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return StatusCode(500, "Error uploading image");
            }
        }

        // POST: api/invitations/upload-music
        [HttpPost("upload-music")]
        public async Task<ActionResult<string>> UploadMusic(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var allowedExtensions = new[] { ".mp3", ".wav", ".ogg" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("Invalid file type");
            }

            try
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                var storedFile = new StoredFile
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Data = memoryStream.ToArray(),
                    Size = file.Length,
                    UploadedAt = DateTime.UtcNow,
                    FileType = "music"
                };

                _context.StoredFiles.Add(storedFile);
                await _context.SaveChangesAsync();

                var fileUrl = $"/api/invitations/files/{storedFile.Id}";
                return Ok(new { url = fileUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading music");
                return StatusCode(500, "Error uploading music");
            }
        }

        // GET: api/invitations/files/{id}
        [HttpGet("files/{id}")]
        public async Task<IActionResult> GetFile(Guid id)
        {
            var file = await _context.StoredFiles.FindAsync(id);

            if (file == null)
            {
                return NotFound();
            }

            return File(file.Data, file.ContentType, file.FileName);
        }

        private static InvitationDto MapToDto(Invitation invitation)
        {
            JsonElement? sections = null;

            if (!string.IsNullOrEmpty(invitation.SectionsJson))
            {
                try
                {
                    sections = JsonSerializer.Deserialize<JsonElement>(invitation.SectionsJson);
                }
                catch
                {
                    // Leave as null if deserialization fails
                }
            }

            return new InvitationDto
            {
                Id = invitation.Id,
                Title = invitation.Title,
                Subtitle = invitation.Subtitle,
                CelebrantName = invitation.CelebrantName,
                EventDate = invitation.EventDate,
                EventTime = invitation.EventTime,
                Venue = invitation.Venue,
                Address = invitation.Address,
                LocationLat = invitation.LocationLat,
                LocationLng = invitation.LocationLng,
                BackgroundColor = invitation.BackgroundColor,
                BackgroundImage = invitation.BackgroundImage,
                TextColor = invitation.TextColor,
                FontFamily = invitation.FontFamily,
                MusicUrl = invitation.MusicUrl,
                Sections = sections,
                FormEmail = invitation.FormEmail,
                CoverEnabled = invitation.CoverEnabled,
                CoverName = invitation.CoverName,
                CoverButtonText = invitation.CoverButtonText,
                CoverBackgroundColor = invitation.CoverBackgroundColor,
                CoverBackgroundImage = invitation.CoverBackgroundImage,
                CoverTextColor = invitation.CoverTextColor,
                CoverButtonColor = invitation.CoverButtonColor,
                CoverButtonTextColor = invitation.CoverButtonTextColor,
                CoverFontFamily = invitation.CoverFontFamily,
                CoverNameFontSize = invitation.CoverNameFontSize,
                CoverButtonFontSize = invitation.CoverButtonFontSize
            };
        }
    }
}
