using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Project.Models
{
    [Table("ClassInformation")]
    public class ClassInformationModel
    {
        [Key]
        public string Id { get;  set; }
        [Required(ErrorMessage = "Class name is required!")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Student count is required!")]
        public int PersonCount { get; set; }

        [StringLength(300, ErrorMessage = "Description can't be longer than 300 characters.")]
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public ClassInformationModel()
        {
        }

        public ClassInformationModel(string className, int personCount, string description) 
        {
            ClassName = className;
            PersonCount = personCount;
            Description = description;
        }
    }
}
