namespace InvitacionesAPI.Models
{
    public class StoredFile
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public long Size { get; set; }
        public DateTime UploadedAt { get; set; }
        public string FileType { get; set; } = string.Empty; // "image" or "music"
    }
}
