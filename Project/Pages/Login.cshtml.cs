using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using Project.Data;

namespace Project.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public LoginModel(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public string UserName { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByNameAsync(UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, Password))
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                var token = Guid.NewGuid().ToString();
                var sessionId = HttpContext.Session.Id;
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("role", user.Role);
                HttpContext.Session.SetString("token", token);
                HttpContext.Session.SetString("session_id", sessionId);
                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };
                Response.Cookies.Append("username", user.UserName, options);
                Response.Cookies.Append("token", token, options);
                Response.Cookies.Append("session_id", sessionId, options);
                return RedirectToPage("/Table");
            }

            ErrorMessage = "Username or password is incorrect.";
            return Page();
        }

        public IActionResult OnGet()
        {
            string sessionUsername = HttpContext.Session.GetString("username");
            string sessionToken = HttpContext.Session.GetString("token");
            string sessionId = HttpContext.Session.GetString("session_id");

            string cookieUsername = Request.Cookies["username"];
            string cookieToken = Request.Cookies["token"];
            string cookieSessionId = Request.Cookies["session_id"];

            if (sessionUsername == null || sessionToken == null || sessionId == null ||
                cookieUsername == null || cookieToken == null || cookieSessionId == null ||
                sessionUsername != cookieUsername || sessionToken != cookieToken || sessionId != cookieSessionId)
            {
                TempData["Error"] = "Unauthorized access.";
            }

            return Page();
        }
    }
}