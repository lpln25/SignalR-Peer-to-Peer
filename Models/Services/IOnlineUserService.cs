using Microsoft.Identity.Client;
using System.Collections.Concurrent;
using WebAppSaba.Models.Entities;
using static WebAppSaba.Models.Services.OnlineUserService;

namespace WebAppSaba.Models.Services
{
    public interface IOnlineUserService
    {
        void AddUser(string userId, string connectionId, string nickName);
        void RemoveUser(string userId);
        IEnumerable<OnlineUserDto> GetOnlineUsers();
        string GetConnectionId(string userId);
    }

    public class OnlineUserService : IOnlineUserService
    {
        private readonly ConcurrentDictionary<string, OnlineUser> _onlineUsers = new();

        public void AddUser(string userId, string connectionId, string nickName)
        {
            _onlineUsers.TryAdd(userId, new OnlineUser
            {
                NickName = nickName
                ,
                ConnectionId = connectionId,
                IsOnline = true
            });
        }

        public void RemoveUser(string userId)
        {
            if (_onlineUsers.TryRemove(userId, out var user))
            {
                user.IsOnline = false;
            }
        }

        public IEnumerable<OnlineUserDto> GetOnlineUsers()
        {
            return _onlineUsers.Where(x => x.Value.IsOnline)
                .Select(x => new OnlineUserDto { UserId = x.Key, Nickname = x.Value.NickName });
        }

        public string GetConnectionId(string userId)
        {
            return _onlineUsers.TryGetValue(userId, out var user) ? user.ConnectionId : null;
        }

        public class OnlineUserDto
        {
            public string UserId { get; set; }
            public string Nickname { get; set; }
        }

    }
}
