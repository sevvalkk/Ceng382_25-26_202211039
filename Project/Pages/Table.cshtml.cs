using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using System.Collections.Generic;
using System.Linq;
using Project.Helpers;
using System.Text.Json;
using Project.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Project.Pages
{
    [Authorize]
    public class TableModel : PageModel
    {
        // public static List<ClassInformationModel> Classes { get; set; } = new List<ClassInformationModel>();
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
        private readonly SchoolDbContext _context;
        private readonly ILogger<TableModel> _logger;
        public TableModel(ILogger<TableModel> logger)
        {
            _logger = logger;
        }
        [ActivatorUtilitiesConstructor]
        public TableModel(SchoolDbContext context, ILogger<TableModel> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IList<ClassInformationModel> ClassList { get; set; }
        // public TableModel()
        // {
        //     if (!Classes.Any())
        //     {
        //         SampleClasses();
        //     }
        // }

        public async Task OnGetAsync(string column)
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

            var query = _context.ClassDB
                .Where(c => c.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(Search))
            {
                var loweredSearch = Search.ToLower();
                query = query.Where(c => c.ClassName.ToLower().Contains(loweredSearch));
            }

            int totalRecords = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

            var pagedClasses = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            FilteredClasses = pagedClasses.Select(c => new ClassInformationTable
            {
                Id = c.Id,
                ClassName = c.ClassName,
                PersonCount = c.PersonCount,
                Description = c.Description,
                IsActive = c.IsActive
            }).ToList();
        }


        public void OnGetToggleColumn(string column)
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

            
            var query = _context.ClassDB
                .Where(c => c.IsActive)
                .AsQueryable();

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
                    PersonCount = c.PersonCount,
                    Description = c.Description,
                    IsActive = c.IsActive
                })
                .ToList();
        }
        

        public IActionResult OnPostAdd()
        {
            var UserName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(UserName))
            {
                return RedirectToPage("/Index");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserName == UserName && u.IsActive);
            if (user == null)
            {
                return RedirectToPage("/Index");
            }

            string newIdValue;
            if (_context.ClassDB.Any())
            {
                int maxNumericId = 0;
                var ids = _context.ClassDB.Select(c => c.Id).ToList();
                foreach (var idStr in ids)
                {
                    if (int.TryParse(idStr, out int numericId))
                    {
                        if (numericId > maxNumericId)
                        {
                            maxNumericId = numericId;
                        }
                    }
                }
                newIdValue = (maxNumericId + 1).ToString();
            }
            else
            {
                newIdValue = "1";
            }

            var newClass = new ClassInformationModel
            {
                Id = newIdValue,
                ClassName = Class.ClassName,
                PersonCount = Class.PersonCount,
                Description = Class.Description,
                IsActive = true,
            };

            _context.ClassDB.Add(newClass);
            _context.SaveChanges();

            return RedirectToPage(new { PageNumber = 1, Search = "" });
        }


        public IActionResult OnPostEdit(string id)
        {
            var classToEdit = _context.ClassDB.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                Class.Id = classToEdit.Id;
                Class.ClassName = classToEdit.ClassName;
                Class.PersonCount = classToEdit.PersonCount;
                Class.Description = classToEdit.Description;
                Class.IsActive = classToEdit.IsActive;
            }
            return Page(); 
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var classToDelete = await _context.ClassDB.FirstOrDefaultAsync(c => c.Id == id);
            if (classToDelete != null)
            {
                classToDelete.IsActive = false;
                await _context.SaveChangesAsync(); 
            }

            return RedirectToPage(new { PageNumber, Search });
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            var updatedClass = await _context.ClassDB.FirstOrDefaultAsync(c => c.Id == Class.Id);
            if (updatedClass != null)
            {
                updatedClass.ClassName = Class.ClassName;
                updatedClass.PersonCount = Class.PersonCount;
                updatedClass.Description = Class.Description;
                updatedClass.IsActive = Class.IsActive;

                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { PageNumber, Search });
        }

        // public List<ClassInformationModel> GetClasses()
        // {
        //     return Classes; 
        // }
        // private void SampleClasses()
        // {
        //     if (Classes.Count == 0) 
        //     {
        //         for (int i = 1; i <= 100; i++)
        //         {
        //             Classes.Add(new ClassInformationModel
        //             {
        //                 Id = i,
        //                 ClassName = $"Class {i}",
        //                 StudentCount = 10 + i,
        //                 Description = $"Description for Class {i}"
        //             });
        //         }
        //     }
        // }
        
        public IActionResult OnPostExportJson(string Search, int PageNumber, List<string> SelectedColumns)
        {

            var query = _context.ClassDB
                .Where(c => c.IsActive)
                .AsQueryable();

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
                    PersonCount = c.PersonCount,
                    Description = c.Description,
                    IsActive = c.IsActive

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
| - UserName : string|
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
▪ UserName
▪ token
▪ session_id (use HttpContext.Session.Id)
▪ Store the same values in cookies using the following cookie settings:
▪ Expires in 30 minutes
▪ HttpOnly = true
▪ Secure = true
▪ SameSite = Strict
Access Control
o On all protected pages, check whether the token, UserName, and session_id from
cookies match those in the session.
o If both token and UserName values match between the session and cookie, then
you may consider the login valid.
o If the check fails, use errors and warnings to say “UserName or password is
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
        public string UserName { get; set; }
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
    "UserName": "admin",
    "Password": "admin123",
    "Role": "Admin",
    "IsActive": true,
    "CreatedAt": "2024-10-01T00:00:00"
  },
  {
    "UserName": "instructor",
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
    public string UserName { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public IActionResult OnPost()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "users.json");
        var users = JsonSerializer.Deserialize<List<User>>(System.IO.File.ReadAllText(filePath));

        var user = users.FirstOrDefault(u => u.UserName == UserName && u.Password == Password && u.IsActive);

        if (user != null)
        {
            var token = Guid.NewGuid().ToString();
            var sessionId = HttpContext.Session.Id;

            // Set session
            HttpContext.Session.SetString("UserName", user.UserName);
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

            Response.Cookies.Append("UserName", user.UserName, options);
            Response.Cookies.Append("token", token, options);
            Response.Cookies.Append("session_id", sessionId, options);

            return RedirectToPage("/Index"); // redirect to class table page
        }

        ErrorMessage = "UserName or password is incorrect.";
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
    string sessionUserName = HttpContext.Session.GetString("UserName");
    string sessionToken = HttpContext.Session.GetString("token");
    string sessionId = HttpContext.Session.GetString("session_id");

    string cookieUserName = Request.Cookies["UserName"];
    string cookieToken = Request.Cookies["token"];
    string cookieSessionId = Request.Cookies["session_id"];

    if (sessionUserName == null || sessionToken == null || sessionId == null ||
        cookieUserName == null || cookieToken == null || cookieSessionId == null ||
        sessionUserName != cookieUserName || sessionToken != cookieToken || sessionId != cookieSessionId)
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

    Response.Cookies.Delete("UserName");
    Response.Cookies.Delete("token");
    Response.Cookies.Delete("session_id");

    return RedirectToPage("/Login");
}
```

---

Would you like a sample `Login.cshtml` page layout too?




PROMPT__________________________
C:\Users\sevva\Desktop\ceng382webproject\Ceng382_25-26_202211039\Project\Pages\Table.cshtml.cs(151,98): error CS0029: Cannot implicitly convert type 'int' to 'string'


        public IActionResult OnPostAdd()
        {
            string nextId = _context.ClassDB.Count() > 0 ? _context.ClassDB.Max(c => c.Id) + 1 : 1;
            var newClass = new ClassInformationModel(Class.ClassName, Class.PersonCount, Class.Description)
            {
                Id = nextId
            };

            _context.ClassDB.Add(newClass);
            return RedirectToPage(new { PageNumber = 1, Search = "" });
        }
RESPONSE______________________

The error CS0029: Cannot implicitly convert type 'int' to 'string' occurs because your code is trying to assign an integer value to a string type without explicit conversion. This happens in the line where nextId is calculated, specifically in the else part of the ternary operator (: 1), and potentially in the Id = nextId assignment if nextId ended up being an integer. Additionally, the way you calculate the maximum ID for string-based numeric IDs is incorrect, and the instantiation of ClassInformationModel needs to use property initializers as no matching constructor exists. You also need to call _context.SaveChanges() to persist the new entity.

Here's the corrected OnPostAdd method:
// ...existing code...
        public IActionResult OnPostAdd()
        {
            string newIdValue;
            if (_context.ClassDB.Any())
            {
                int maxNumericId = 0;
                var ids = _context.ClassDB.Select(c => c.Id).ToList(); // Get all string IDs
                foreach (var idStr in ids)
                {
                    if (int.TryParse(idStr, out int numericId))
                    {
                        if (numericId > maxNumericId)
                        {
                            maxNumericId = numericId;
                        }
                    }
                    // Optionally, you could log a warning or handle cases where an Id is not a parseable integer.
                }
                newIdValue = (maxNumericId + 1).ToString();
            }
            else
            {
                newIdValue = "1"; // Start with "1" if no classes exist
            }

            var newClass = new ClassInformationModel
            {
                Id = newIdValue,
                ClassName = Class.ClassName,
                PersonCount = Class.PersonCount,
                Description = Class.Description,
                IsActive = true // Assuming new classes should be active by default
            };

            _context.ClassDB.Add(newClass);
            _context.SaveChanges(); // Persist changes to the database

            return RedirectToPage(new { PageNumber = 1, Search = "" });
        }
// ...existing code...






*/