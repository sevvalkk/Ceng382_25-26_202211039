using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class ClassInformationTable 
    {
        [Key]
        public string Id { get;  set; }

        public string ClassName { get; set; }

        public int PersonCount { get; set; }

        public string Description { get; set; }
        
        [Required]
        public bool IsActive { get; set; }
    }
}
