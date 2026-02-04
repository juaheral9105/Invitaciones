namespace InvitacionesAPI.Services
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(string toEmail, string guestName, string eventName, bool willAttend, int numberOfGuests);
        Task SendFormConfirmationEmailAsync(string toEmail, string phone, Dictionary<string, string> formData);
    }
}
