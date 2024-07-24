using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters")]
        public string? Password { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? PasswordConfirmation { get; set; }


        public string? Description { get; set; }
        public string? Avatar { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
    }
}
