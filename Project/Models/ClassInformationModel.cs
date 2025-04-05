using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class ClassInformationModel
    {
        public int Id { get;  set; }
        [Required(ErrorMessage = "Class name is required!")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Student count is required!")]
        public int StudentCount { get; set; }

        [StringLength(300, ErrorMessage = "Description can't be longer than 300 characters.")]
        public string? Description { get; set; }

        public ClassInformationModel()
        {
        }

        public ClassInformationModel(string className, int studentCount, string description) 
        {
            ClassName = className;
            StudentCount = studentCount;
            Description = description;
        }
    }
}
