namespace InvitacionesAPI.Models
{
    public class GuestListItem
    {
        public int Id { get; set; }

        // Foreign key to Invitation
        public Guid InvitationId { get; set; }
        public Invitation? Invitation { get; set; }

        // Guest information
        public string Name { get; set; } = string.Empty;

        // Phone is the unique identifier for guest validation
        public string Phone { get; set; } = string.Empty;

        // Number of guests this person can bring
        public int NumberOfGuests { get; set; }

        // Names of accompanying guests (comma-separated or JSON)
        public string? GuestNames { get; set; }

        // Event configuration
        public bool EventHasAccommodation { get; set; }

        // Permission for this specific guest
        public bool AccommodationAllowed { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
