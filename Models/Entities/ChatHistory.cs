namespace WebAppSaba.Models.Entities
{
    /// <summary>
    /// تاریخچه ارتباط بین دو کاربر
    /// </summary>
    public class ChatHistory
    {
        public long Id { get; set; }
        public string SenderId { get; set; }
        public string ReciverId { get; set; }
        public DateTime DateTimeCreated { get; set; }
    }
}
