using System.ComponentModel.DataAnnotations;

namespace WebAppSaba.Models.ViewModel
{
    public class UserSigninVM
    {
        [Display(Name = "اسم مستعار")]
        public string? Nickname { get; set; }
        [Display(Name = "نام کاربری")]
        public string Username { get; set; }
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }
    }
}
