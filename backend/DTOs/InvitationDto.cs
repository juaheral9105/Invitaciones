using System.Text.Json;

namespace InvitacionesAPI.DTOs
{
    public class InvitationDto
    {
        public Guid? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? CelebrantName { get; set; }
        public DateTime? EventDate { get; set; }
        public string? EventTime { get; set; }
        public string? Venue { get; set; }
        public string? Address { get; set; }
        public decimal? LocationLat { get; set; }
        public decimal? LocationLng { get; set; }
        public string BackgroundColor { get; set; } = "#ffffff";
        public string? BackgroundImage { get; set; }
        public string TextColor { get; set; } = "#000000";
        public string FontFamily { get; set; } = "Arial";
        public string? MusicUrl { get; set; }
        public JsonElement? Sections { get; set; }  // Changed to JsonElement to accept any JSON structure
        public string? FormEmail { get; set; }

        // Cover Page Configuration
        public bool CoverEnabled { get; set; } = true;
        public string CoverName { get; set; } = "Nombre de la Homenajeada";
        public string CoverButtonText { get; set; } = "Ver Invitación";
        public string CoverBackgroundColor { get; set; } = "#f3e5f5";
        public string? CoverBackgroundImage { get; set; }
        public string CoverTextColor { get; set; } = "#ffffff";
        public string CoverButtonColor { get; set; } = "#ffffff";
        public string CoverButtonTextColor { get; set; } = "#000000";
        public string CoverFontFamily { get; set; } = "Playfair Display";
        public string CoverNameFontSize { get; set; } = "6rem";
        public string CoverButtonFontSize { get; set; } = "1.125rem";
    }

    public class SectionDto
    {
        public string Type { get; set; } = string.Empty; // text, image, list, map
        public string? Title { get; set; }
        public string? Content { get; set; }
        public List<string>? Items { get; set; }
        public string? ImageUrl { get; set; }
        public int Order { get; set; }
    }
}
