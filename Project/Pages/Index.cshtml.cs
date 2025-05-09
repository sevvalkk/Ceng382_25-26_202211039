using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            // Optionally, you can add redirection logic here if needed.
        }
    }
}


// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;
// using Microsoft.EntityFrameworkCore;
// using Project.Models;
// using Project.Data;
// using Microsoft.AspNetCore.Authorization;

// namespace Project.Pages
// {
//     [Authorize]
//     public class IndexModel : PageModel
//     {
//         private readonly SchoolDbContext _context;

//         public IndexModel(SchoolDbContext context)
//         {
//             _context = context;
//         }

//         [BindProperty]
//         public string UserName { get; set; }

//         [BindProperty]
//         public string Password { get; set; }

//         public string ErrorMessage { get; set; }

//         public async Task<IActionResult> OnPostAsync()
//         {
//             var user = await _context.Users
//                 .FirstOrDefaultAsync(u => u.UserName == UserName && u.PasswordHash == Password && u.IsActive);

//             if (user != null)
//             {
//                 var token = Guid.NewGuid().ToString();
//                 var sessionId = HttpContext.Session.Id;

//                 HttpContext.Session.SetString("UserName", user.UserName);
//                 HttpContext.Session.SetString("role", user.Role);
//                 HttpContext.Session.SetString("token", token);
//                 HttpContext.Session.SetString("session_id", sessionId);

//                 CookieOptions options = new CookieOptions
//                 {
//                     Expires = DateTime.Now.AddMinutes(30),
//                     HttpOnly = true,
//                     Secure = true,
//                     SameSite = SameSiteMode.Strict
//                 };

//                 Response.Cookies.Append("UserName", user.UserName, options);
//                 Response.Cookies.Append("token", token, options);
//                 Response.Cookies.Append("session_id", sessionId, options);

//                 return RedirectToPage("/Table");
//             }

//             ErrorMessage = "Username or password is incorrect.";
//             return Page();
//         }

//         public IActionResult OnGet()
//         {
//             string sessionUserName = HttpContext.Session.GetString("UserName");
//             string sessionToken = HttpContext.Session.GetString("token");
//             string sessionId = HttpContext.Session.GetString("session_id");

//             string cookieUserName = Request.Cookies["UserName"];
//             string cookieToken = Request.Cookies["token"];
//             string cookieSessionId = Request.Cookies["session_id"];

//             if (sessionUserName == null || sessionToken == null || sessionId == null ||
//                 cookieUserName == null || cookieToken == null || cookieSessionId == null ||
//                 sessionUserName != cookieUserName || sessionToken != cookieToken || sessionId != cookieSessionId)
//             {
//                 TempData["Error"] = "Unauthorized access.";
//             }

//             return Page();
//         }

//         public IActionResult OnPostLogout()
//         {
//             HttpContext.Session.Clear();

//             Response.Cookies.Delete("UserName");
//             Response.Cookies.Delete("token");
//             Response.Cookies.Delete("session_id");

//             return RedirectToPage("/Index");
//         }
//     }
// }



/*
    AI-----------------
    PROMPT:_________________________________

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Project.Models;
    using System.Collections.Generic;
    using System.Linq;

    namespace Project.Pages
    {
        public class IndexModel : PageModel
        {

            [BindProperty]
            public List<ClassInformationModel> Classes { get; set; } = new List<ClassInformationModel>();
            public ClassInformationModel Class { get; set; } = new ClassInformationModel();

            public void OnGet()
            {
                if (Classes == null)
                {
                    Classes = new List<ClassInformationModel>();
                }
            }


            public IActionResult OnPostAdd()
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                Classes.Add(Class);

                Class = new ClassInformationModel();

                return RedirectToPage();
            }

            public IActionResult OnPostEdit(int id)
            {
                var classToEdit = Classes.FirstOrDefault(c => c.Id == id);
                if (classToEdit != null)
                {
                    Class = new ClassInformationModel
                    {
                        Id = classToEdit.Id,
                        ClassName = classToEdit.ClassName,
                        StudentCount = classToEdit.StudentCount,
                        Description = classToEdit.Description
                    };
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
        }
    }

    it doesnt add a class to the class table


    RESPONSE:________________________________

    Why Isn't the Class Being Added?
    Your issue is that Classes isn't persisting across requests.

    Since Razor Pages recreates the PageModel on each request, Classes is being reinitialized every time a request is made, meaning previously added classes are lost.

    ✅ Solution: Use a Persistent Data Source
    To fix this, you need to store Classes somewhere persistent, like:

    A Static List (for testing purposes)

    A Database (for real applications)

    Session State (temporary solution)

    Option 1: Use a Static List (Quick Fix)
    Modify IndexModel.cs:

    public class IndexModel : PageModel
    {
        // ✅ Static List - Keeps data in memory across requests
        private static List<ClassInformationModel> Classes = new List<ClassInformationModel>();

        [BindProperty]
        public ClassInformationModel Class { get; set; } = new ClassInformationModel();

        public void OnGet()
        {
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Classes.Add(new ClassInformationModel
            {
                ClassName = Class.ClassName,
                StudentCount = Class.StudentCount,
                Description = Class.Description
            });

            return RedirectToPage();
        }

        public List<ClassInformationModel> GetClasses()
        {
            return Classes; // ✅ Use this method in Razor to display classes
        }
    }
    Modify Index.cshtml to Use GetClasses()

    @foreach (var c in Model.GetClasses())

*/


/*

PROMPT:_______________________________
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class ClassInformationModel
    {
        private static int counter = 1;
        public int Id { get;  set; }
        [Required(ErrorMessage = "Class name is required!")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Student count is required!")]
        public int StudentCount { get; set; }

        [StringLength(300, ErrorMessage = "Description can't be longer than 300 characters.")]
        public string? Description { get; set; }

        public ClassInformationModel()
        {
            Id = GetNextId(); 
        }

        private static int GetNextId()
        {
            return counter++; 
        }

        public ClassInformationModel(string className, int studentCount, string description) : this()
        {
            ClassName = className;
            StudentCount = studentCount;
            Description = description;

        }
    }
}

why it doesnt autoincrement properly, it starts from 4 and increases 4 by 4

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Class.Id = Classes.Count > 0 ? Classes.Max(c => c.Id) + 1 : 1;

            Classes.Add(new ClassInformationModel(Class.ClassName, Class.StudentCount, Class.Description));

            return RedirectToPage();
        }

is that because of this


RESPONSE:_________________________________
Yes! Your issue is caused by this line:

Class.Id = Classes.Count > 0 ? Classes.Max(c => c.Id) + 1 : 1;
🔍 What's Happening?
You manually set Class.Id before creating a new ClassInformationModel instance.

Then, you call the constructor of ClassInformationModel, which has its own ID generation logic (counter++).

Because of this double assignment, the IDs are not sequential as expected.

✅ Correcting the Issue
Option 1: Remove Manual ID Assignment
Let ClassInformationModel handle the ID itself.

public IActionResult OnPostAdd()
{
    if (!ModelState.IsValid)
    {
        return Page();
    }

    // Directly create a new instance (it will auto-assign the ID correctly)
    var newClass = new ClassInformationModel(Class.ClassName, Class.StudentCount, Class.Description);
    Classes.Add(newClass);

    return RedirectToPage();
}
✅ Why This Works?

ClassInformationModel will assign Id using the static counter properly.

No unnecessary override of Id.

Option 2: Use the List to Determine the Next ID
If you want Id to be based on the list instead of a static counter:

public IActionResult OnPostAdd()
{
    if (!ModelState.IsValid)
    {
        return Page();
    }

    int nextId = Classes.Count > 0 ? Classes.Max(c => c.Id) + 1 : 1;
    var newClass = new ClassInformationModel(Class.ClassName, Class.StudentCount, Class.Description)
    {
        Id = nextId // Assign manually
    };

    Classes.Add(newClass);
    return RedirectToPage();
}
✅ Why This Works?
Ensures IDs are sequential even if some entries are deleted.
No static counter needed, so every instance only depends on the list.

🔹 Which Option Should You Use?
If IDs should persist across page reloads → Use a database (this is the best long-term solution).
If you only need IDs to be unique per session → Option 1 (let the constructor handle it).
If you want IDs to always be sequential and start from 1 after a restart → Option 2.
*/

/*
PROMPT:____________________________________
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using System.Collections.Generic;
using System.Linq;

namespace Project.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> Classes { get; set; } = new List<ClassInformationModel>();
        
        [BindProperty]
        public ClassInformationModel Class { get; set; } = new ClassInformationModel();

        public void OnGet()
        {
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            int nextId = Classes.Count > 0 ? Classes.Max(c => c.Id) + 1 : 1;
            var newClass = new ClassInformationModel(Class.ClassName, Class.StudentCount, Class.Description)
            {
                Id = nextId 
            };

            Classes.Add(newClass);
            return RedirectToPage();
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
        public IActionResult OnPostUpdate(int id)
        {
            var UpdatedClass = Classes.FirstOrDefault(c => c.Id == id);
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
    }
}

@page
@model Project.Pages.IndexModel

<div class="container" style="justify-content:center; top:center; margin-top: 20%;">
    <div class="row">
        <div class="col-md-6">
            <h2 style="font-family: Impact, sans-serif;">Add or Edit Class</h2>

            <form method="post">
                <div class="form-group">
                    <label for="ClassName">Class Name</label>
                    <input type="text" id="ClassName" asp-for="Class.ClassName" class="form-control" required />
                </div>

                <div class="form-group">
                    <label for="StudentCount">Student Count</label>
                    <input type="number" id="StudentCount" asp-for="Class.StudentCount" class="form-control" required />
                </div>

                <div class="form-group">
                    <label for="Description">Description</label>
                    <textarea id="Description" asp-for="Class.Description" class="form-control" required></textarea>
                </div>

                <button type="submit" class="btn btn-success" asp-page-handler="Add">Add Class</button>
                <button type="submit" class="btn btn-warning" asp-page-handler="Update">Update</button>
            </form>

        </div>

        <div class="col-md-6">
            <h2 style="font-family: Impact, sans-serif;">Class List</h2>
            <table class="table table-striped table-hover">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Class Name</th>
                        <th>Student Count</th>
                        <th>Description</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    @foreach (var c in Model.GetClasses())
                    {
                        <tr>
                            <td>@c.Id</td>
                            <td>@c.ClassName</td>
                            <td>@c.StudentCount</td>
                            <td>@c.Description</td>
                            <td>
                                <div >

                                    <form method="post" >
                                        <button type="submit" class="btn btn-warning" asp-page-handler="Edit" asp-route-id="@c.Id">Edit</button>
                                    </form>
                                    <form method="post">
                                        <button type="submit" class="btn btn-danger" asp-page-handler="Delete" asp-route-id="@c.Id">Delete</button>
                                    </form>
                                </div>
                            </td>
                        </tr>
                    }

                </tbody>
            </table>
        </div>
    </div>
</div>


why it doesnt update anything 


RESPONSE:___________________________________

Your update functionality is not working because of the way you're handling data in the OnPostUpdate method. Here are a few potential issues and how to fix them:
Form Does Not Provide ID for Update:

The form used for adding and updating a class does not include a hidden input field for the Id. This means when you click "Update," there's no Id associated with the Class model, so it does not know which class to update.
Fix the OnPostUpdate Method in IndexModel.cs
Modify the OnPostUpdate method to check for a valid Id:

public IActionResult OnPostUpdate()
{
    var existingClass = Classes.FirstOrDefault(c => c.Id == Class.Id);
    if (existingClass != null)
    {
        existingClass.ClassName = Class.ClassName;
        existingClass.StudentCount = Class.StudentCount;
        existingClass.Description = Class.Description;
    }
    return RedirectToPage();
}
Fix Explanation:

Instead of taking id as a parameter, it now directly updates Class.Id, which will be set from the form.





PROMPT:______________________________
@page
@model Project.Pages.IndexModel

    <div class="container" style="width:1000px; margin-top:50px;">
        <div class="row" >
            <div class="col-md-4 container" style="height:700px; width:500px; justify-content:center; background: linear-gradient(to right, rgb(31, 126, 167), rgb(76, 154, 200, 0.5)); border-radius:15px; box-shadow: 0 0 10px #000; overflow-y: auto;">
                <h2 class="text-center" style="font-family: Impact, sans-serif; text-decoration:underline; color:rgb(232, 248, 255);">-Class Information-</h2>

                <form method="post" class="container text-center" style="margin-top:40%;" >
                    <input type="hidden" asp-for="Class.Id" />
                    <div class="form-group">
                        <label for="ClassName" style="font-weight: bold; font-family: 'Times New Roman', Times, serif; font-size: 20px; color:rgb(232, 248, 255);">Class Name</label>
                        <input type="text" id="ClassName" asp-for="Class.ClassName" class="form-control" required />
                    </div>

                    <div class="form-group">
                        <label for="StudentCount" style="font-weight: bold; font-family: 'Times New Roman', Times, serif; font-size: 20px; color:rgb(232, 248, 255);">Student Count</label>
                        <input type="number" id="StudentCount" asp-for="Class.StudentCount" class="form-control" required />
                    </div>

                    <div class="form-group">
                        <label for="Description" style="font-weight: bold; font-family: 'Times New Roman', Times, serif; font-size: 20px; color:rgb(232, 248, 255);">Description</label>
                        <textarea id="Description" asp-for="Class.Description" class="form-control" style="max-height: 200px;" required></textarea>
                    </div>
                    <div style="display:flex; justify-content:center; margin-top:50px;">
                        <button type="submit" class="btn btn-success" asp-page-handler="Add" style="font-size: 15px;">Add Class</button>
                        <button type="submit" class="btn btn-warning" asp-page-handler="Update" style="font-size: 15px">Update</button>
                    </div>
                </form>

            </div>

            <div class="col-md-7" style="height:700px; justify-content:center; background: linear-gradient(to right, rgb(76, 154, 200, 0.5), rgb(31, 126, 167)); border-radius:15px; box-shadow: 0 0 10px #000; overflow-y: auto;">
                <h2 style="font-family: Impact, sans-serif; text-decoration:underline; color:rgb(232, 248, 255);" class="text-center">-Class List-</h2>
                
                <form method="get" class="form-inline" style="margin-top:20px; display:flex; justify-content:center;">
                    <div class="form-group">
                        <label for="Search" class="mr-2"></label>
                        <input type="text" id="Search" name="Search" class="form-control" placeholder="Class Name" value="@Model.Search" />
                        <button type="submit" class="btn btn-info ml-2" style="background-color:rgb(28, 108, 143);">Filter</button>
                    </div>
                </form>
                
                <table class="table table-striped table-hover" style=" margin-top:30px; ">
                    <thead>
                        <tr style="font-family: 'Times New Roman', Times, serif; font-size: 18px; color:rgb(232, 248, 255);">
                            <th>
                                <!-- Form for toggling column selection for ClassName -->
                                <form method="get">
                                    <input type="hidden" name="Search" value="@Model.Search" />
                                    <input type="hidden" name="PageNumber" value="@Model.PageNumber" />
                                    <button name="column" value="ClassName" class="btn @((Model.SelectedColumns.Contains("ClassName") ? "btn-info" : "btn-outline-secondary"))">Class Name</button>
                                </form>
                            </th>
                            <th>
                                <!-- Form for toggling column selection for StudentCount -->
                                <form method="get">
                                    <input type="hidden" name="Search" value="@Model.Search" />
                                    <input type="hidden" name="PageNumber" value="@Model.PageNumber" />
                                    <button name="column" value="StudentCount" class="btn @((Model.SelectedColumns.Contains("StudentCount") ? "btn-info" : "btn-outline-secondary"))">Student Count</button>
                                </form>
                            </th>
                            <th>
                                <!-- Form for toggling column selection for Description -->
                                <form method="get">
                                    <input type="hidden" name="Search" value="@Model.Search" />
                                    <input type="hidden" name="PageNumber" value="@Model.PageNumber" />
                                    <button name="column" value="Description" class="btn @((Model.SelectedColumns.Contains("Description") ? "btn-info" : "btn-outline-secondary"))">Description</button>
                                </form>
                            </th>
                            <th>Actions</th>
                        </tr>
                    </thead>


                    <tbody>
                        @if (Model.FilteredClasses != null && Model.FilteredClasses.Any())
                        {
                            @foreach (var c in Model.FilteredClasses)
                            {
                                <tr>
                                    <td>@c.ClassName</td>
                                    <td>@c.StudentCount</td>
                                    <td>@c.Description</td>
                                    <td>
                                        <div>
                                            <form method="post">
                                                <button type="submit" class="btn btn-warning" asp-page-handler="Edit" asp-route-id="@c.Id">
                                                    <i class="bi bi-pencil"></i>
                                                </button>
                                            </form>
                                            <form method="post">
                                                <button type="submit" class="btn btn-danger" asp-page-handler="Delete" asp-route-id="@c.Id">
                                                    <i class="bi bi-trash"></i>
                                                </button>
                                            </form>
                                        </div>
                                    </td>
                                </tr>
                            }
                        }
                        else
                        {
                            <tr>
                                <td colspan="5" class="text-center">No classes found.</td>
                            </tr>
                        }


                    </tbody>
                </table>


                <!-- Pagination -->
                <div class="d-flex justify-content-center">
                    @if (Model.PageNumber > 1)
                    {
                        <a style="color:rgb(232, 248, 255);" href="?PageNumber=@(Model.PageNumber - 1)&Search=@Model.Search" class="btn btn-link">Previous</a>
                    }

                    @for (int i = 1; i <= Model.TotalPages; i++)
                    {
                        <a style="color:rgb(232, 248, 255);"href="?PageNumber=@i&Search=@Model.Search" class="btn btn-link @((Model.PageNumber == i) ? "active" : "")">@i</a>
                    }

                    @if (Model.PageNumber < Model.TotalPages)
                    {
                        <a style="color:rgb(232, 248, 255);"href="?PageNumber=@(Model.PageNumber + 1)&Search=@Model.Search" class="btn btn-link">Next</a>
                    }
                </div>

            </div>
           <form method="post" style="margin-top:20px; display:flex; justify-content:center;">
                @foreach (var col in Model.SelectedColumns)
                {
                    <input type="hidden" name="SelectedColumns" value="@col" />
                }
                <input type="hidden" name="Search" value="@Model.Search" />

                <!-- Single export button that handles both filtered and unfiltered data -->
                <button type="submit" asp-page-handler="ExportJson" class="btn btn-primary">
                    Export @((string.IsNullOrEmpty(Model.Search)) ? "Unfiltered" : "Filtered")
                </button>
            </form>

        </div>
    </div>



 public IActionResult OnPostExportJson(string ExportMode)
        {
            List<ClassInformationTable> source;

            // If there is a search term, use FilteredClasses, else use full Classes
            if (!string.IsNullOrEmpty(Search) && FilteredClasses != null && FilteredClasses.Any())
            {
                source = FilteredClasses;
            }
            else
            {
                source = Classes.Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                }).ToList();
            }

            if (SelectedColumns == null || !SelectedColumns.Any())
            {
                SelectedColumns = new List<string> { "Id", "ClassName", "StudentCount", "Description" };
            }

            string exportFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Exports", "Json", $"export_{(string.IsNullOrEmpty(Search) ? "unfiltered" : "filtered")}_{DateTime.Now:yyyyMMdd_HHmmss}.json");

            Utils.Instance.ExportToJson(source, exportFilePath, SelectedColumns);

            return RedirectToPage(new { ExportedFilePath = exportFilePath });
        }


public void OnGet(string column)
        {
            // Toggle column selection if necessary
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


using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Project.Helpers
{
    public class Utils
    {
        private static readonly Lazy<Utils> _instance = new Lazy<Utils>(() => new Utils());

        public static Utils Instance => _instance.Value;

        public void ExportToJson<T>(List<T> data, string filePath, List<string> selectedColumns = null)
        {
            var filteredData = new List<Dictionary<string, object>>();

            foreach (var item in data)
            {
                var dict = new Dictionary<string, object>();

                // If no specific columns are selected, export all properties
                if (selectedColumns == null || selectedColumns.Count == 0)
                {
                    foreach (var property in item.GetType().GetProperties())
                    {
                        dict[property.Name] = property.GetValue(item);
                    }
                }
                else
                {
                    // Export only selected columns
                    foreach (var property in item.GetType().GetProperties())
                    {
                        if (selectedColumns.Contains(property.Name))
                        {
                            dict[property.Name] = property.GetValue(item);
                        }
                    }
                }

                filteredData.Add(dict);
            }

            var json = JsonSerializer.Serialize(filteredData, new JsonSerializerOptions { WriteIndented = true });

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, json);
        }
    }
}


sayfa ilk fotoğraftaki gibi, filtreleyince de sonraki gibi oluyor. ben istiyorum ki sadece tabloda görünen classlar export etsin






RESPONSE:______________________________
Anladım! Yani senin isteğin şu:

> **Filtrelenmiş olsa bile**, sadece **görüntülenen sayfadaki sınıflar** (yani tabloda o anda görünen kayıtlar) `Export` butonuna basıldığında `JSON` olarak dışa aktarılmalı.

### 🔧 Şu Anki Durum
Senin `OnPostExportJson` handler'ın **tüm filtrelenmiş sonuçları** (yani `FilteredClasses`) alıp hepsini export ediyor.

Ama **sen sadece sayfalama sonucu tabloda görünenleri** export etmek istiyorsun.

---

### ✅ Çözüm

Senin `FilteredClasses` zaten sayfalanmış verileri tutuyor. Yani `OnGet` içinde `.Skip(...).Take(...)` yaptığın yerde sayfalanan veri orada oluşuyor.

Ama `OnPostExportJson` metodunda `FilteredClasses`'ı kullanman için bu listenin yeniden elde edilmesi gerekiyor, çünkü `OnPost` çağrısı sırasında `FilteredClasses` dolu olmayabilir (sayfalamayı da hesaba katmak lazım).

Bu yüzden önerim:

1. `OnPostExportJson` metodunu **gösterilen tablo verisi kadar veriyle sınırlayacak** hale getir.
2. Gerekirse, `PageNumber`, `Search`, ve `SelectedColumns` bilgilerini formdan da gönder.

### 🛠️ Adım Adım Değişiklik

#### 1. Export butonundaki forma gerekli `PageNumber` bilgisini de ekle:

<form method="post" style="margin-top:20px; display:flex; justify-content:center;">
    @foreach (var col in Model.SelectedColumns)
    {
        <input type="hidden" name="SelectedColumns" value="@col" />
    }
    <input type="hidden" name="Search" value="@Model.Search" />
    <input type="hidden" name="PageNumber" value="@Model.PageNumber" />

    <button type="submit" asp-page-handler="ExportJson" class="btn btn-primary">
        Export @((string.IsNullOrEmpty(Model.Search)) ? "Unfiltered" : "Filtered")
    </button>
</form>
```

#### 2. `OnPostExportJson` metodunu güncelle (yalnızca mevcut sayfadaki kayıtlar alınacak):

public IActionResult OnPostExportJson(string Search, int PageNumber, List<string> SelectedColumns)
{
    var query = Classes.AsQueryable();

    if (!string.IsNullOrEmpty(Search))
    {
        query = query.Where(c => c.ClassName.Contains(Search, StringComparison.OrdinalIgnoreCase));
    }

    // Sayfalanan veriyi al (şu an tabloda görünenler)
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


### ✅ Artık Ne Oldu?

- Filtre varsa: uygulanır.
- Sayfa numarası alınır: sadece o sayfadaki kayıtlar alınır.
- `Export` işlemi sadece tabloda görünen kayıtları içerir 🎯



*/