using WebAppSaba.Contexts;
using WebAppSaba.Models.Entities;
using static WebAppSaba.Models.Services.OnlineUserService;

namespace WebAppSaba.Models.Services
{
    public interface IChatHistoryService
    {
        Task<bool> HasChatHistoryWithUser(string reciverId, string senderId);
        Task Save(ChatHistory chatHistory);
        List<HistoryUserDto> GetChatHistoryUser(string userId);

    }

    public class ChatHistoryService : IChatHistoryService
    {

        private readonly DatabaseContext _db;
        public ChatHistoryService(DatabaseContext db)
        {
            _db = db;
        }

        public List<HistoryUserDto> GetChatHistoryUser(string userId)
        {
            var chatHistories = _db.ChatHistories.Where(c => c.SenderId == userId)
                .Select(c => c.ReciverId).ToList();

            var users = _db.Users.Where(u => chatHistories.Contains(u.Id.ToString()))
                .Select(u => new HistoryUserDto() { Id = u.Id.ToString(), Nickname = u.Nickname})
                .ToList();

            return users;
        }

        public Task<bool> HasChatHistoryWithUser(string reciverId, string senderId)
        {
            var findChatHistory = _db.ChatHistories.FirstOrDefault(c => c.ReciverId == reciverId && c.SenderId == senderId);
            if (findChatHistory != null)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);

        }

        public Task Save(ChatHistory chatHistory)
        {
            _db.ChatHistories.Add(chatHistory);
            _db.SaveChanges();
            return Task.CompletedTask;
        }


    }

    public class HistoryUserDto
    {
        public string Id { get; set; }
        public string Nickname { get; set; }
    }

}
