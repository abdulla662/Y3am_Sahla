using sahla.DTOs.Response;

namespace sahla.Controllers.Admin.UserControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> usermanger)
        {
            _unitOfWork = unitOfWork;
            _userManager = usermanger;
        }
        [HttpGet("allusers")]
        public IActionResult allusers()
        {
            var users = _userManager.Users.ToList().Count();
            return Ok(users);
        }
        [HttpGet("GetAllCourses")]
        public IActionResult GetAllCourses()
        {
            var courses = _unitOfWork.Courses.Get().ToList().Count();
            if (courses == null)
                return NotFound("No courses found");
            return Ok(courses);
        }
        [HttpGet("getalluserslist")]
        public async Task<IActionResult> GetAllUsersList([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _unitOfWork.ApplicationUsers.Get();

            var totalUsers = query.Count();
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            var users = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var usersDto = users.Adapt<List<UserDto>>();
            for (int i = 0; i < users.Count; i++)
            {
                var roles = await _userManager.GetRolesAsync(users[i]);
                usersDto[i].Role = roles.FirstOrDefault() ?? "Student";
            }
            for (int i = 0; i < usersDto.Count; i++)
            {
                usersDto[i].IsBlocked = users[i].LockoutEnabled && users[i].LockoutEnd > DateTime.UtcNow;
            }
            var response = new
            {
                currentPage = page,
                totalPages,
                users = usersDto
            };

            return Ok(response);
        }
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> EditUser([FromQuery] string id, [FromBody] UpdateUserDto userDto)
        {
            var currentUser = _unitOfWork.ApplicationUsers.GetOne(e => e.Id == id);
            if (currentUser == null)
                return NotFound("User not found");

            userDto.Adapt(currentUser);

            currentUser.NormalizedUserName = currentUser.UserName?.ToUpper();
            currentUser.NormalizedEmail = currentUser.Email?.ToUpper();

            if (!string.IsNullOrWhiteSpace(userDto.Password))
            {
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                currentUser.PasswordHash = passwordHasher.HashPassword(currentUser, userDto.Password);
            }

            _unitOfWork.ApplicationUsers.Edit(currentUser);
            await _unitOfWork.CompleteAsync();

            return Ok("User updated successfully");
        }

        [HttpGet("getbyid")]
        public IActionResult GetById(string id)
        {
            var user = _unitOfWork.ApplicationUsers.GetOne(u => u.Id == id);
            if (user == null) return NotFound();
            var dto = user.Adapt<UpdateUserDto>();
            return Ok(dto);
        }

        [HttpPut("ChangeRole")]
        public async Task<IActionResult> ChangeRole([FromQuery] string id, [FromQuery] string role)
        {
            var user = _unitOfWork.ApplicationUsers.GetOne(e => e.Id == id);
            var currnetuser = _userManager.GetUserId(User);
            if (currnetuser == id) {
                return BadRequest("❌ You cannot change your own role.");
            }
            if (user == null)
                return NotFound("User not found");

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                    return BadRequest("Failed to remove current roles");
            }

            var addResult = await _userManager.AddToRoleAsync(user, role);
            if (!addResult.Succeeded)
                return BadRequest("Failed to assign new role");

            return Ok($"✅ Role changed to {role}");
        }
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromQuery] string id)
        {
            var user = _unitOfWork.ApplicationUsers.GetOne(e => e.Id == id);
            if (user == null)
                return NotFound("User not found");
            var currentUser = _userManager.GetUserId(User);
            if (currentUser == id)
            {
                return BadRequest("❌ You cannot delete your own account.");
            }
            _unitOfWork.ApplicationUsers.Delete(user);
            await _unitOfWork.CompleteAsync();
            return Ok("✅ User deleted successfully");
        }
        [HttpPut("BlockUser")]
        public async Task<IActionResult> BlockUser([FromQuery] string id,DateTime time)
        {
            var user = _unitOfWork.ApplicationUsers.GetOne(e => e.Id == id);
            if (user == null)
                return NotFound("User not found");
            user.LockoutEnabled = true;
            user.LockoutEnd = time;
            _unitOfWork.ApplicationUsers.Edit(user);
            await _unitOfWork.CompleteAsync();
            return Ok("✅ User blocked successfully");
        }
        [HttpPut("UnlockUser")]
        public async Task<IActionResult> UnlockUser([FromQuery] string id)
        {
            var user = _unitOfWork.ApplicationUsers.GetOne(e => e.Id == id);
            if (user == null)
                return NotFound("User not found");

            user.LockoutEnabled = false;
            user.LockoutEnd = null;

            _unitOfWork.ApplicationUsers.Edit(user);
            await _unitOfWork.CompleteAsync();

            return Ok("✅ User unblocked successfully");
        }
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return BadRequest(new { error = "Email is already in use." });

            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                Adress = request.Address
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Student"); // Role افتراضي
                return Ok(new { message = "User created successfully", userId = user.Id });
            }

            return BadRequest(result.Errors);
        }






    }
}
