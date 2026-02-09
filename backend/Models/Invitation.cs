using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvitacionesAPI.Models
{
    public class Invitation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Subtitle { get; set; }

        [MaxLength(200)]
        public string? CelebrantName { get; set; }

        public DateTime? EventDate { get; set; }

        [MaxLength(100)]
        public string? EventTime { get; set; }

        [MaxLength(300)]
        public string? Venue { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [Column(TypeName = "decimal(10,7)")]
        public decimal? LocationLat { get; set; }

        [Column(TypeName = "decimal(10,7)")]
        public decimal? LocationLng { get; set; }

        [MaxLength(50)]
        public string BackgroundColor { get; set; } = "#ffffff";

        [MaxLength(500)]
        public string? BackgroundImage { get; set; }

        [MaxLength(50)]
        public string TextColor { get; set; } = "#000000";

        [MaxLength(100)]
        public string FontFamily { get; set; } = "Arial";

        [MaxLength(500)]
        public string? MusicUrl { get; set; }

        [Column(TypeName = "jsonb")]
        public string? SectionsJson { get; set; }

        [MaxLength(200)]
        public string? FormEmail { get; set; }

        // Cover Page Configuration
        public bool CoverEnabled { get; set; } = true;

        [MaxLength(200)]
        public string CoverName { get; set; } = "Nombre de la Homenajeada";

        [MaxLength(100)]
        public string CoverButtonText { get; set; } = "Ver Invitación";

        [MaxLength(50)]
        public string CoverBackgroundColor { get; set; } = "#f3e5f5";

        [MaxLength(500)]
        public string? CoverBackgroundImage { get; set; }

        [MaxLength(50)]
        public string CoverTextColor { get; set; } = "#ffffff";

        [MaxLength(50)]
        public string CoverButtonColor { get; set; } = "#ffffff";

        [MaxLength(50)]
        public string CoverButtonTextColor { get; set; } = "#000000";

        [MaxLength(100)]
        public string CoverFontFamily { get; set; } = "Playfair Display";

        [MaxLength(50)]
        public string CoverNameFontSize { get; set; } = "6rem";

        [MaxLength(50)]
        public string CoverButtonFontSize { get; set; } = "1.125rem";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public ICollection<Confirmation> Confirmations { get; set; } = new List<Confirmation>();
    }
}
