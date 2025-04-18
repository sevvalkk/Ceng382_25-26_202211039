using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using System.Collections.Generic;
using System.Linq;
using Project.Helpers;
using System.Text.Json;


namespace Project.Pages
{
    public class TableModel : PageModel
    {
    public static List<ClassInformationModel> Classes { get; set; } = new List<ClassInformationModel>();
        private const int PageSize = 10;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        public int TotalPages { get; set; }
        public List<ClassInformationTable> FilteredClasses { get; set;} 
        [BindProperty]
        public ClassInformationModel Class { get; set; } = new ClassInformationModel();
        
        [BindProperty(SupportsGet = true)]
        public List<string> SelectedColumns { get; set; } = new List<string>();
        private static DateTime _lastSearchTime = DateTime.MinValue;


        public TableModel()
        {
            if (!Classes.Any())
            {
                SampleClasses();
            }
        }
        
        public void OnGet(string column)
        {
            if (!string.IsNullOrEmpty(column))
            {
                var columnsFromQuery = Request.Query["SelectedColumns"].ToString();
                var currentSelection = columnsFromQuery?.Split(',').ToList() ?? new List<string>();

                if (currentSelection.Contains(column))
                    currentSelection.Remove(column);
                else
                    currentSelection.Add(column);

                SelectedColumns = currentSelection;
            }

            if (PageNumber <= 0)
                PageNumber = 1;

            // Apply search filter if search is not null or empty
            var query = Classes.AsQueryable();
            if (!string.IsNullOrEmpty(Search))
            {
                query = query.Where(c => c.ClassName.Contains(Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalRecords = query.Count();
            TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

            FilteredClasses = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                })
                .ToList();
        }
        


        public IActionResult OnPostAdd()
        {

            int nextId = Classes.Count > 0 ? Classes.Max(c => c.Id) + 1 : 1;
            var newClass = new ClassInformationModel(Class.ClassName, Class.StudentCount, Class.Description)
            {
                Id = nextId 
            };

            Classes.Add(newClass);
            return RedirectToPage(new { PageNumber = 1, Search = "" });
        }


        public IActionResult OnPostEdit(int id)
        {
            var classToEdit = Classes.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                Class.Id = classToEdit.Id;
                Class.ClassName = classToEdit.ClassName;
                Class.StudentCount = classToEdit.StudentCount;
                Class.Description = classToEdit.Description;
            }
            return Page(); 
        }

        public IActionResult OnPostDelete(int id)
        {
            var classToDelete = Classes.FirstOrDefault(c => c.Id == id);
            if (classToDelete != null)
            {
                Classes.Remove(classToDelete);
            }

            return RedirectToPage(); 
        }
        public IActionResult OnPostUpdate()
        {
            var UpdatedClass = Classes.FirstOrDefault(c => c.Id == Class.Id);
            if (UpdatedClass != null)
            {
                UpdatedClass.ClassName = Class.ClassName;
                UpdatedClass.StudentCount = Class.StudentCount;
                UpdatedClass.Description = Class.Description;
            }

            return RedirectToPage(); 
        }
        public List<ClassInformationModel> GetClasses()
        {
            return Classes; 
        }
        private void SampleClasses()
        {
            if (Classes.Count == 0) 
            {
                for (int i = 1; i <= 100; i++)
                {
                    Classes.Add(new ClassInformationModel
                    {
                        Id = i,
                        ClassName = $"Class {i}",
                        StudentCount = 10 + i,
                        Description = $"Description for Class {i}"
                    });
                }
            }
        }
        
        public IActionResult OnPostExportJson(string Search, int PageNumber, List<string> SelectedColumns)
        {
            var query = Classes.AsQueryable();

            if (!string.IsNullOrEmpty(Search))
            {
                query = query.Where(c => c.ClassName.Contains(Search, StringComparison.OrdinalIgnoreCase));
            }

            var pagedData = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                })
                .ToList();

            if (SelectedColumns == null || !SelectedColumns.Any())
            {
                SelectedColumns = new List<string> { "Id", "ClassName", "StudentCount", "Description" };
            }

            string exportFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Exports", "Json",
                $"export_page_{PageNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.json");

            Utils.Instance.ExportToJson(pagedData, exportFilePath, SelectedColumns);

            return RedirectToPage(new { ExportedFilePath = exportFilePath });
        }

    }
}






/*
AI:

PROMPT__________________________
User Information Source
o Store user login information in a file named users.json located under
wwwroot/data/.
o You must define a User class (Models/User.cs) to match the structure of this JSON
file.
o Below is the UML representation of the User class:
+--------------------+
| User |
+--------------------+
| - Username : string|
| - Password : string|
| - Role : string|
| - IsActive : bool |
| - CreatedAt: DateTime |
+--------------------+
Login Functionality
o When the login form is submitted, read the users from the JSON file.
o Check whether the given credentials match an active user in the list.
Session and Cookies
o Upon successful login:
▪ Generate a simple token
▪ Store the following in the session:
▪ username
▪ token
▪ session_id (use HttpContext.Session.Id)
▪ Store the same values in cookies using the following cookie settings:
▪ Expires in 30 minutes
▪ HttpOnly = true
▪ Secure = true
▪ SameSite = Strict
Access Control
o On all protected pages, check whether the token, username, and session_id from
cookies match those in the session.
o If both token and username values match between the session and cookie, then
you may consider the login valid.
o If the check fails, use errors and warnings to say “username or password is
incorrect.” Or something like this message.
Logout
o Create a logout button that clears the session and removes all cookies related to
login.
o Upon successful logout, the user should be redirected to the login page.
Redirection after Login
o Upon successful login, the user should be redirected to the table page you
implemented last week. This redirection should be implemented in the OnPostAsync
method inside Login.cshtml.cs

RESPONSE______________________
Great! You’re implementing a secure login system with session and cookie handling. Here's how you can structure your implementation step-by-step based on what you've described:

### ✅ **1. Create the `User` class**
**Path:** `Models/User.cs`
```csharp
using System;

namespace Project.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
```

---

### ✅ **2. Sample `users.json` in `wwwroot/data/`**
```json
[
  {
    "Username": "admin",
    "Password": "admin123",
    "Role": "Admin",
    "IsActive": true,
    "CreatedAt": "2024-10-01T00:00:00"
  },
  {
    "Username": "instructor",
    "Password": "pass456",
    "Role": "Instructor",
    "IsActive": true,
    "CreatedAt": "2025-01-10T12:30:00"
  }
]
```

---

### ✅ **3. Login Logic (`Login.cshtml.cs`)**
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System;

public class LoginModel : PageModel
{
    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public IActionResult OnPost()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "users.json");
        var users = JsonSerializer.Deserialize<List<User>>(System.IO.File.ReadAllText(filePath));

        var user = users.FirstOrDefault(u => u.Username == Username && u.Password == Password && u.IsActive);

        if (user != null)
        {
            var token = Guid.NewGuid().ToString();
            var sessionId = HttpContext.Session.Id;

            // Set session
            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("session_id", sessionId);

            // Set cookie
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("username", user.Username, options);
            Response.Cookies.Append("token", token, options);
            Response.Cookies.Append("session_id", sessionId, options);

            return RedirectToPage("/Index"); // redirect to class table page
        }

        ErrorMessage = "Username or password is incorrect.";
        return Page();
    }
}
```

---

### ✅ **4. Session Validation for Protected Pages**
Add this check to `OnGet()` or `OnGetAsync()` of protected pages:

```csharp
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
        return RedirectToPage("/Login");
    }

    // Proceed with page logic
    return Page();
}
```

---

### ✅ **5. Logout Handler**
**In your logout page or button handler:**

```csharp
public IActionResult OnPostLogout()
{
    HttpContext.Session.Clear();

    Response.Cookies.Delete("username");
    Response.Cookies.Delete("token");
    Response.Cookies.Delete("session_id");

    return RedirectToPage("/Login");
}
```

---

Would you like a sample `Login.cshtml` page layout too?

*/