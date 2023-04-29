using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Identity.Client;
using System.Security.Claims;
using WebAppSaba.Models.Entities;
using WebAppSaba.Models.Services;

namespace WebAppSaba.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IUserService _userService;
        private readonly IChatMessageService _chateMessageService;
        public readonly IChatFileService _chatFileService;
        public readonly IChatHistoryService _chatHistoryService;
        private readonly IOnlineUserService _onlineUserService;
        public ChatHub(IUserService userService, IChatMessageService chateMessageService
            , IChatFileService chatFileService, IChatHistoryService chatHistoryService
            , IOnlineUserService onlineUserService)
        {
            _userService = userService;
            _chateMessageService = chateMessageService;
            _chatFileService = chatFileService;
            _chatHistoryService = chatHistoryService;
            _onlineUserService = onlineUserService;

        }
        public async override Task OnConnectedAsync()
        {


            string userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            string nickName = Context.User.FindFirstValue(ClaimTypes.Name);
            if (!string.IsNullOrEmpty(userId))
            {
                _onlineUserService.AddUser(userId, Context.ConnectionId, nickName);
                await Clients.All.SendAsync("UpdateOnlineUsers", _onlineUserService.GetOnlineUsers());
                await Clients.User(userId).SendAsync("UpdateArciveChatHistory", _chatHistoryService.GetChatHistoryUser(userId));

            }




            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _onlineUserService.RemoveUser(userId);
                await Clients.All.SendAsync("UpdateOnlineUsers", _onlineUserService.GetOnlineUsers());
            }


            await base.OnDisconnectedAsync(exception);
        }


        public async Task LoadMessages(string reciverId)
        {
            // آیا تاریخچه داریم؟
            string senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (reciverId == null || reciverId == senderId) return;
            if (senderId != null)
            {
                bool checkHistory = await _chatHistoryService.HasChatHistoryWithUser(reciverId, senderId);
                // خیر
                if (checkHistory == false)
                {
                    await MakeNewChat(reciverId);
                    return;
                }
                // بله
                var ChatMessages = await _chateMessageService.GetChatMessages(reciverId, senderId);

                await Clients.User(senderId).SendAsync("GetLoadMessages", ChatMessages);
            }
        }

        /// <summary>
        /// ارسال پیام کاربر به کاربر
        /// </summary>
        /// <param name="receiverId">آیدی فرستنده</param>
        /// <param name="message">پیام متنی</param>
        ///// <param name="imageData"></param>
        ///// <param name="fileName"></param>
        ///// <param name="fileData"></param>
        /// <returns></returns>
        public async Task SendMessage(string reciverId, string message/*, string imageData, string fileName, string fileData*/)
        {

            string senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            User sender = _userService.Get(senderId);

            var receiver = _userService.Get(reciverId);

            if (sender == null || receiver == null)
            {
                return;
            }

            var chatMessage = new ChatMessage
            {
                Nickname = sender.Nickname,
                SenderId = sender.Id.ToString(),
                ReciverId = reciverId,
                Message = message,
                Timestamp = DateTime.UtcNow.AddHours(3.5)
            };

            //if (!string.IsNullOrEmpty(imageData))
            //{
            //    chatMessage.ImageData = Convert.FromBase64String(imageData);
            //}

            //if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(fileData))
            //{
            //    chatMessage.FileData = Convert.FromBase64String(fileData);
            //}

            await _chateMessageService.Save(chatMessage);

            // اگر آنلاین بود براش پیام بفرست
            if (_onlineUserService.GetConnectionId(reciverId) != null)
            {
                await Clients.User(reciverId).SendAsync("ReceiveMessage", chatMessage /*, imageData, fileName, fileData*/);
            }
            await Clients.User(senderId).SendAsync("ReceiveMessage", chatMessage /*, imageData, fileName, fileData*/);
        }





        /// <summary>
        /// ایجاد مکالمه جدید بین دو کاربر
        /// </summary>
        /// <param name="receiverId"></param>
        /// <param name="senderId"></param>
        /// <returns></returns>
        private async Task StartChatHistory(string receiverId, string senderId, DateTime Timestamp)
        {
            ChatHistory chatHistory1 = new ChatHistory()
            {
                DateTimeCreated = Timestamp,
                ReciverId = receiverId,
                SenderId = senderId,
            };

            ChatHistory chatHistory2 = new ChatHistory()
            {
                DateTimeCreated = DateTime.UtcNow.AddHours(3.5),
                ReciverId = senderId,
                SenderId = receiverId,
            };
            await _chatHistoryService.Save(chatHistory1);
            await _chatHistoryService.Save(chatHistory2);
        }

        public async Task SendFile(string receiverId, string fileName, string fileData)
        {
            string SenderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            User sender = _userService.Get(SenderId);
            var receiver = _userService.Get(receiverId);

            if (sender == null || receiver == null)
            {
                return;
            }

            var file = new ChatFile
            {
                SenderId = sender.Id.ToString(),
                ReceiverId = receiverId,
                FileName = fileName,
                FileData = Convert.FromBase64String(fileData),
                Timestamp = DateTime.UtcNow
            };

            await _chatFileService.Save(file);

            await Clients.User(receiverId).SendAsync("ReceiveFile", sender.Nickname, fileName, fileData, file.Timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        }

        /// <summary>
        /// آغاز مکالمه
        /// </summary>
        /// <param name="receiverId">آیدی فرستنده</param>
        /// <returns></returns>
        public async Task MakeNewChat(string reciverId)
        {
            string SenderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            string SenderNickname = Context.User.FindFirstValue(ClaimTypes.Name);
            string message = $" آغاز مکالمه از طرف {SenderNickname}";
            DateTime Timestamp = DateTime.UtcNow.AddHours(3.5);
            if (SenderId == reciverId) return;
            // ثبت تاریخچه
            await StartChatHistory(reciverId, SenderId, Timestamp);

            // ثبت اولین پیام
            await SendMessage(reciverId, message);

            // ثبت تاریخچه پیام
            await Clients.User(SenderId).SendAsync("UpdateArciveChatHistory", _chatHistoryService.GetChatHistoryUser(SenderId));
            if (_onlineUserService.GetConnectionId(reciverId) != null)// اگر آنلاین بود براش پیام بفرست
                await Clients.User(reciverId).SendAsync("UpdateArciveChatHistory", _chatHistoryService.GetChatHistoryUser(reciverId));
        }


    }
}
