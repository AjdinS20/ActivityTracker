using ActivityTracker.Data;
using ActivityTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ActivityTracker.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public UserService(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApplicationUser> GetUserProfileAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<ApplicationUser> DeleteUserProfileAsync(string email)
        {
            var user =  await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
            _context.Users.Remove(user);
            _context.SaveChanges();
            return user;
        }

        public async Task<string> GetUserRegionCodeAsync(string ipAddress)
        {
            // Using a free IP geolocation service (e.g., ipinfo.io or ipapi.co)
            var httpClient = _httpClientFactory.CreateClient();
            var url = $"https://ipapi.co/{ipAddress}/json/";

            try
            {
                var response = await httpClient.GetFromJsonAsync<GeolocationResponse>(url);
                if (response != null)
                {
                    // Define Balkan countries ISO codes
                    var balkanCountries = new HashSet<string> { "AL", "BA", "BG", "HR", "GR", "XK", "MK", "ME", "RS", "SI" };
                    if (balkanCountries.Contains(response.CountryCode))
                    {
                        return "BA"; // Code for Balkan region
                    }
                }
            }
            catch (Exception)
            {
                // Handle or log exceptions if needed
            }

            return "EN"; // Default to English if not in the Balkans
        }

        private class GeolocationResponse
        {
            public string CountryCode { get; set; } // This field will map to the ISO country code returned by the geolocation API
        }
    }
}
