using ActivityTracker.Models;
using ActivityTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ActivityTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserService userService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var email = User.FindFirstValue(ClaimTypes.Name);

            var user = await _userService.GetUserProfileAsync(email);
            if (user == null) return NotFound();

            var profile = new
            {
                user.Email,
                user.Weight,
                user.Height,
                user.YearOfBirth,
                user.Gender
            };

            return Ok(profile);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteProfile()
        {
            var email = User.FindFirstValue(ClaimTypes.Name);

            var user = await _userService.GetUserProfileAsync(email);
            if (user == null) return NotFound();
            var profile = new
            {
                user.Email,
                user.Weight,
                user.Height,
                user.YearOfBirth,
                user.Gender
            };
            await _userService.DeleteUserProfileAsync(email);

            return Ok(profile);
        }

        [HttpGet("region")]
        public async Task<IActionResult> GetRegion()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(ipAddress))
            {
                return BadRequest("Unable to determine IP address.");
            }

            var regionCode = await _userService.GetUserRegionCodeAsync(ipAddress);
            return Ok(new { RegionCode = regionCode });
        }
    }
}
