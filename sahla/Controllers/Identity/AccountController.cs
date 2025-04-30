using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace sahla.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private  readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManger;
        private readonly IConfiguration _configuration;



        public AccountController(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManger,SignInManager<ApplicationUser> signInManager,RoleManager<IdentityRole>  roleManager, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManger;
            _signInManager = signInManager;
            _roleManger = roleManager;
            _configuration = configuration;

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            ApplicationUser applicationUser=registerRequest.Adapt<ApplicationUser>();
            var result=await _userManager.CreateAsync(applicationUser, registerRequest.password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(applicationUser, false);
                if (!_roleManger.Roles.Any())
                {
                    await _roleManger.CreateAsync(new IdentityRole("SuperAdmin"));
                    await _roleManger.CreateAsync(new IdentityRole("Admin"));
                    await _roleManger.CreateAsync(new IdentityRole("Teacher"));
                    await _roleManger.CreateAsync(new IdentityRole("Employee"));
                    await _roleManger.CreateAsync(new IdentityRole("Assistant"));
                    await _roleManger.CreateAsync(new IdentityRole("Student"));

                }
                await _userManager.AddToRoleAsync(applicationUser, "Student");
                return Created();
            }
            else
            {
                ModelStateDictionary keyvaluepairs = new();
                return BadRequest(result.Errors);
            }

        }
        [HttpPost("Login")]
        public async Task<IActionResult> login([FromBody] LoginRequest loginRequest)
        {

            var appuser = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (appuser != null)
            {
                var result = await _userManager.CheckPasswordAsync(appuser, loginRequest.Password);


                if (result == true)
                {
                    await _signInManager.SignInAsync(appuser, loginRequest.RememberMe);



                    var token = GenerateJwtToken(appuser);
                    return Ok(new { token });
                }
                else
                {
                    ModelStateDictionary keyvaluepairs = new();
                    keyvaluepairs.AddModelError("Error", "Data is Wrong");
                    return BadRequest(keyvaluepairs);
                }
            }
            else return NotFound("User Not Found");

        }
        [Authorize]
        [HttpGet("logout")]
        public IActionResult logout()
        {
            _signInManager.SignOutAsync();
            return NoContent();
        }
        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpGet("google-login")]
        public IActionResult GoogleLogin(string returnUrl = "/")
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account", new { ReturnUrl = returnUrl }, Request.Scheme, Request.Host.ToString());
            Console.WriteLine("Redirect URL: " + redirectUrl); // log it
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return BadRequest("Error loading external login information.");

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (signInResult.Succeeded)
            {

                return Ok("Google login successful");
            }

            var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
                return BadRequest("Email claim not found");

            var user = new ApplicationUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, isPersistent: false);
            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

    }




}







