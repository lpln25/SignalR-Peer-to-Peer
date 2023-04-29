using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppSaba.Models.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string SenderId { get; set; }
        public string ReciverId { get; set; }
        public string Message { get; set; }
        //public byte[] ImageData { get; set; }
        //public byte[] FileData { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
