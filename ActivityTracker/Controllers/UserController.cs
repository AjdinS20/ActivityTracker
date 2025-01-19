using ActivityTracker.Services;
using Microsoft.AspNetCore.Authorization;
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

        public UserController(UserService userService)
        {
            _userService = userService;
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
