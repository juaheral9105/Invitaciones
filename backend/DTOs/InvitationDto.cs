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
        public List<SectionDto>? Sections { get; set; }
        public string? FormEmail { get; set; }
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
