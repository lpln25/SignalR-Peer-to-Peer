using WebAppSaba.Contexts;
using WebAppSaba.Models.Entities;
using WebAppSaba.Models.ViewModel;

namespace WebAppSaba.Models.Services
{
    public interface IUserService
    {
        bool Exist(User user);
        User Login(UserLoginVM user);
        User Add(User user);
        User Get(string Id);

    }

    public class UserService : IUserService
    {
        private readonly DatabaseContext _db;
        public UserService(DatabaseContext db)
        {
            _db = db;
        }
        public User Add(User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
            return user;
        }

        public User Login(UserLoginVM user)
        {
            var findUser = _db.Users.SingleOrDefault(u => u.Username == user.Username
            && u.Password == user.Password);
            if (findUser != null)
            {
                return findUser;
            }
            return null;
        }

        public bool Exist(User user)
        {
            var findUser = _db.Users.SingleOrDefault(u => u.Username == user.Username);
            if (findUser != null)
            {
                return true;
            }
            return false;
        }

        public User Get(string Id)
        {
            long id = 0;
            try { id = Convert.ToInt64(Id); }
            catch { return null; }

            var findUser = _db.Users.SingleOrDefault(u => u.Id == id);
            if (findUser != null)
            {
                return findUser;
            }
            return null;
        }

        
    }
}
