using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace WebAppSaba.Models.Entities
{
    public class User
    {

        public long Id { get; set; }
        [Display(Name = "اسم مستعار")]
        public string Nickname { get; set; }
        [Display(Name = "نام کاربری")]
        public string Username { get; set; }
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        public ICollection<ChatMessage> SentMessages { get; set; }
        public ICollection<ChatMessage> ReceivedMessages { get; set; }
    }
}
