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
        public static List<ClassInformationModel> Classes { get; set; } = new List<ClassInformationModel>();
        public ClassInformationModel NewClass { get; set; } = new ClassInformationModel();

        public void OnGet()
        {
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Classes.Add(NewClass);

            NewClass = new ClassInformationModel();

            return RedirectToPage(); // Refresh the page
        }

        public IActionResult OnPostEdit(int id)
        {
            var classToEdit = Classes.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                NewClass = new ClassInformationModel
                {
                    Id = classToEdit.Id,
                    ClassName = classToEdit.ClassName,
                    StudentCount = classToEdit.StudentCount,
                    Description = classToEdit.Description
                };
            }
            return Page(); // Reload the page with data populated for editing
        }

        public IActionResult OnPostUpdate()
        {
            var classToUpdate = Classes.FirstOrDefault(c => c.Id == NewClass.Id);
            if (classToUpdate != null)
            {
                classToUpdate.ClassName = NewClass.ClassName;
                classToUpdate.StudentCount = NewClass.StudentCount;
                classToUpdate.Description = NewClass.Description;
            }

            return RedirectToPage(); // Refresh the page after update
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
