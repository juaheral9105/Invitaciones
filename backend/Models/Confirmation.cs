using System.ComponentModel.DataAnnotations;

namespace InvitacionesAPI.Models
{
    public class Confirmation
    {
        [Key]
        public Guid Id { get; set; }

        public Guid InvitationId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required]
        public bool WillAttend { get; set; }

        public int NumberOfGuests { get; set; } = 1;

        [MaxLength(1000)]
        public string? Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Invitation Invitation { get; set; } = null!;
    }
}
