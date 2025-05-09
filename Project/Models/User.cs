using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Project.Models
{
    [Table("Users")]
    public class User : IdentityUser
    {
        // [Key]
        // public string UserId { get; set; }

        // [NotMapped] 
        // public string Username { get; set; }

        // [NotMapped] 
        // public string Password { get; set; }

        public string Role { get; set; } = "User";
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}