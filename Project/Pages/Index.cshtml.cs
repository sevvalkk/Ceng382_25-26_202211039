using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using System.Collections.Generic;
using System.Linq;
using Project.Helpers;

namespace Project.Pages
{
    public class IndexModel : PageModel
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


        public IndexModel()
        {
            if (!Classes.Any())
            {
                SampleClasses();
            }
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




    }
    
}


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
*/