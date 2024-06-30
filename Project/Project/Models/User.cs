using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Areas.Identity.Data
{
    public class User : IdentityUser
    {
        [Column(TypeName = "varchar(18)")]
        public string Name { get; set; }

        [Column(TypeName = "text")]
        public string? Description { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string? Avatar { get; set; }
    }
}