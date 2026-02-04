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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public ICollection<Confirmation> Confirmations { get; set; } = new List<Confirmation>();
    }
}
