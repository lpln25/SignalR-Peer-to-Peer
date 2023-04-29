using WebAppSaba.Contexts;
using WebAppSaba.Models.Entities;

namespace WebAppSaba.Models.Services
{
    public interface IChatFileService
    {
        Task Save(ChatFile cahtFile);
    }

    public class ChatFileService : IChatFileService
    {
        private readonly DatabaseContext _db;
        public ChatFileService(DatabaseContext db)
        {
            _db = db;
        }
        public Task Save(ChatFile cahtFile)
        {
            _db.ChatFiles.Add(cahtFile);
            _db.SaveChanges();
            return Task.CompletedTask;
        }
    }
}
