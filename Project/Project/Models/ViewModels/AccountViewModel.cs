using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models.ViewModels
{
    public class AccountViewModel
    {
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? Email { get; set; }

        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters")]
        public string? Password { get; set; }

        [NotMapped]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? PasswordConfirmation { get; set; }
    }

}

