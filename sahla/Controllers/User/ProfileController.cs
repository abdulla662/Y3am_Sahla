using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sahla.Models;

namespace sahla.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProfileController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManger;
        }
        [Authorize]
        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.Adress,
                user.Points,
                CreatedAt = user.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd")
            });
        }
        [Authorize]
        [HttpPost("Update_Picture")]
        public async Task<IActionResult> UpdatePicture(IFormFile? profilePicture, [FromForm] string? address)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_images");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            if (!string.IsNullOrEmpty(user.ProfilePicture))
            {
                var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfilePicture.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            if (profilePicture != null && profilePicture.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(profilePicture.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }

                user.ProfilePicture = $"/profile_images/{fileName}";
            }

            if (!string.IsNullOrWhiteSpace(address))
            {
                user.Adress = address;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("User update failed");
            }

            return Ok("Profile updated successfully");
        }

        [HttpGet("Get_Picture")]
        public async Task<IActionResult> GetPicture()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            if (string.IsNullOrEmpty(user.ProfilePicture))
                return NotFound("No profile picture found");

            return Ok(new { pictureUrl = user.ProfilePicture });
        }
    }
}


