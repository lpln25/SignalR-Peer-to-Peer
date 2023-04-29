using WebAppSaba.Contexts;
using WebAppSaba.Models.Entities;

namespace WebAppSaba.Models.Services
{
    public interface IChatMessageService
    {
        Task<List<ChatMessage>> GetChatMessages(string reciverId, string senderId);
        Task Save(ChatMessage chatMessage);
    }

    public class ChatMessageService : IChatMessageService
    {
        private readonly DatabaseContext _db;
        public ChatMessageService(DatabaseContext db)
        {
            _db = db;
        }

        public Task<List<ChatMessage>> GetChatMessages(string reciverId, string senderId)
        {
            List<ChatMessage> allMessages = new List<ChatMessage>();
            var listMessageSender = _db.ChatMessages.Where(c => c.ReciverId == reciverId && c.SenderId == senderId).ToList();
            var listMessageReciver = _db.ChatMessages.Where(c => c.ReciverId == senderId && c.SenderId == reciverId).ToList();
            allMessages.AddRange(listMessageSender);
            allMessages.AddRange(listMessageReciver);
            return Task.FromResult(allMessages.OrderBy(c => c.Timestamp).ToList());
        }

        public Task Save(ChatMessage chatMessage)
        {
            _db.ChatMessages.Add(chatMessage);
            _db.SaveChanges();
            return Task.CompletedTask;
        }
    }
}
