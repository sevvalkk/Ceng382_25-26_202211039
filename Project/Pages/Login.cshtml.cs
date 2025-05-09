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
                HttpContext.Session.SetString("UserName", user.UserName);
                return RedirectToPage("/Table");
            }

            ErrorMessage = "Username or password is incorrect.";
            return Page();
        }
    }
}