using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAppSaba.Models.Entities;

namespace WebAppSaba.Controllers
{
    [Authorize]
    public class ChatRoomController : Controller
    {
        public IActionResult Index()
        {
            User model = new User()
            {
                Nickname = User.FindFirstValue(ClaimTypes.Name),
            };

            return View(model);
        }
    }
}
