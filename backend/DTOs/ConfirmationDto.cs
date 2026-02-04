namespace InvitacionesAPI.DTOs
{
    public class ConfirmationDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool WillAttend { get; set; }
        public int NumberOfGuests { get; set; } = 1;
        public string? Message { get; set; }
    }
}
