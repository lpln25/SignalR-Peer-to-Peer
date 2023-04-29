using Microsoft.EntityFrameworkCore;
using WebAppSaba.Models.Entities;

namespace WebAppSaba.Contexts
{
    public class DatabaseContext: DbContext
    {
        public DatabaseContext(DbContextOptions options):base(options) { }

        public DbSet<User> Users { get; set; } 
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatHistory> ChatHistories { get; set; }
        public DbSet<ChatFile> ChatFiles { get; set; }
    }
}
