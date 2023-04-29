using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WebAppSaba.Models;
using WebAppSaba.Models.Entities;
using WebAppSaba.Models.Services;
using WebAppSaba.Models.ViewModel;

namespace WebAppSaba.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "ChatRoom");
            }

            return View();
        }

        public IActionResult Login(UserLoginVM user)
        {
            var findUser = _userService.Login(user);
            if (findUser != null)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, findUser.Nickname),
                    new Claim(ClaimTypes.NameIdentifier, findUser.Id.ToString()),
                    new Claim(ClaimTypes.GivenName, findUser.Username)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var properties = new AuthenticationProperties
                {
                    RedirectUri = Url.Content("/chatroom/Index")
                };
                return SignIn(new ClaimsPrincipal(identity),
                    properties, CookieAuthenticationDefaults.AuthenticationScheme);
            }
            else
                return View(user);
        }

        [HttpGet]
        public IActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signin(UserSigninVM user)
        {
            bool checkError = false;
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            if (user.Nickname == null || user.Nickname == string.Empty)
            {
                ModelState.AddModelError(nameof(UserSigninVM.Nickname), "اسم مستعار نباید خالی باشه");
                checkError = true;
            }
            if (_userService.Exist(new User() { Username = user.Username, Password = user.Password }) == true)
            {
                ModelState.AddModelError(nameof(UserSigninVM.Username), " نام کاربری تکراری");
                checkError = true;
            }

            if (checkError) { return View(user); }
            else
            {
                // register User
                var newUser = _userService.Add(new User()
                {
                    Nickname = user.Nickname,
                    Username = user.Username,
                    Password = user.Password
                });
                // login User
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, newUser.Nickname),
                    new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                    new Claim(ClaimTypes.GivenName, newUser.Username)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var properties = new AuthenticationProperties
                {
                    RedirectUri = Url.Content("/chatroom/Index")
                };
                return SignIn(new ClaimsPrincipal(identity),
                    properties, CookieAuthenticationDefaults.AuthenticationScheme);
            }

        }

        public IActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                var properties = new AuthenticationProperties
                {
                    RedirectUri = Url.Content("/home/login")
                };
                return SignOut(properties);
            }
            else
                return RedirectToAction("Login");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}