using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityTracker.Models
{
    public class RegistrationModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? ConfirmPassword { get; set; }
        public string? Weight { get; set; }
        public string? Height { get; set; }
        public string? YearOfBirth { get; set; }
        public bool? Gender { get; set; }
    }
}
