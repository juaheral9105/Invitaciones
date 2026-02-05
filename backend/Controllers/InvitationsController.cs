using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvitacionesAPI.Data;
using InvitacionesAPI.Models;
using InvitacionesAPI.DTOs;
using InvitacionesAPI.Services;
using System.Text.Json;

namespace InvitacionesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvitationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InvitationsController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly ICloudinaryService _cloudinaryService;

        public InvitationsController(
            ApplicationDbContext context,
            ILogger<InvitationsController> logger,
            IWebHostEnvironment environment,
            ICloudinaryService cloudinaryService)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
            _cloudinaryService = cloudinaryService;
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
                SectionsJson = dto.Sections != null ? JsonSerializer.Serialize(dto.Sections) : null,
                FormEmail = dto.FormEmail,
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
            invitation.SectionsJson = dto.Sections != null ? JsonSerializer.Serialize(dto.Sections) : null;
            invitation.FormEmail = dto.FormEmail;
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
            try
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(file);
                return Ok(new { url = imageUrl });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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
            try
            {
                var musicUrl = await _cloudinaryService.UploadMusicAsync(file);
                return Ok(new { url = musicUrl });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading music");
                return StatusCode(500, "Error uploading music");
            }
        }

        private static InvitationDto MapToDto(Invitation invitation)
        {
            List<SectionDto>? sections = null;

            if (!string.IsNullOrEmpty(invitation.SectionsJson))
            {
                try
                {
                    sections = JsonSerializer.Deserialize<List<SectionDto>>(invitation.SectionsJson);
                }
                catch
                {
                    sections = new List<SectionDto>();
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
                FormEmail = invitation.FormEmail
            };
        }
    }
}
