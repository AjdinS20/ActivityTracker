using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ActivityTrackerData.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? Weight { get; set; }
        public string? Height { get; set; }
        public string? YearOfBirth { get; set; }
        public bool? Gender {  get; set; }
    }
}
