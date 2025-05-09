using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("UserName");
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("session_id");
            return RedirectToPage("/Logout");
        }
    }
}
