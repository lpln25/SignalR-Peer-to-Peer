namespace WebAppSaba.Models.Entities
{
    public class ChatFile
    {
        public Guid Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
