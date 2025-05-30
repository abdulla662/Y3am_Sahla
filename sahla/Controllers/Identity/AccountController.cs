﻿using Newtonsoft.Json;
using System.Threading.Tasks;

        namespace sahla.Controllers.Identity
        {
            [Route("api/[controller]")]
            [ApiController]

            public class AccountController : ControllerBase
            {
                private readonly IUnitOfWork _unitOfWork;
                private readonly UserManager<ApplicationUser> _userManager;
                private readonly SignInManager<ApplicationUser> _signInManager;
                private readonly RoleManager<IdentityRole> _roleManger;
                private readonly IConfiguration _configuration;
                private readonly IEmailSender _emailSender;




                public AccountController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManger, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailSender emailSender)
                {
                    _unitOfWork = unitOfWork;
                    _userManager = userManger;
                    _signInManager = signInManager;
                    _roleManger = roleManager;
                    _configuration = configuration;
                    _emailSender = emailSender;


                }
                [HttpPost("register")]
                public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
                {
                    ApplicationUser applicationUser = registerRequest.Adapt<ApplicationUser>();
                    var result = await _userManager.CreateAsync(applicationUser, registerRequest.password);
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
                    return CreatedAtAction(nameof(Register), new { id = applicationUser.Id }, new { message = "User registered", userId = applicationUser.Id });
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
                // ✅ تحقق إذا كان محظور
                if (appuser.LockoutEnd != null && appuser.LockoutEnd > DateTime.UtcNow)
                {
                    return BadRequest(new { isBlocked = true, until = appuser.LockoutEnd.Value.ToString("yyyy-MM-dd HH:mm") });
                }

                var result = await _userManager.CheckPasswordAsync(appuser, loginRequest.Password);
                if (result == true)
                {
                    await _signInManager.SignInAsync(appuser, loginRequest.RememberMe);
                    var token = await GenerateJwtToken(appuser);
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
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return NoContent();
    }
            private async Task<string> GenerateJwtToken(ApplicationUser user)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AbdullahosyHosnyHasona$abdullahamdy$hosnyhasona$abdullahosny$hamoda$y3amshla"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                   issuer: "https://localhost:7273",
                   audience: "http://127.0.0.1:5500",  
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            [HttpGet("google-login")]
            public IActionResult GoogleLogin()
            {
                var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                    new { returnUrl = "http://127.0.0.1:5500/sahla-frontend/User/html/home.html" },
                    protocol: Request.Scheme);

                var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
                return Challenge(properties, "Google");
            }

            [HttpGet("ExternalLoginCallback")]
            public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null) return BadRequest("Error loading external login information.");

                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

                ApplicationUser user;
                if (result.Succeeded)
                {
                    user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
                {
                    var blockMessage = Uri.EscapeDataString(JsonConvert.SerializeObject(new
                    {
                        isBlocked = true,
                        until = user.LockoutEnd.Value.ToString("yyyy-MM-dd HH:mm")
                    }));
                    return Redirect($"{returnUrl}?token={blockMessage}");
                }
            }
                else
                {
                    var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;
                    if (email == null) return BadRequest("Email claim not found");

                    user = new ApplicationUser { UserName = email, Email = email };
                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded) return BadRequest(createResult.Errors);

                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                }

                var token = await GenerateJwtToken(user);
                return Redirect($"{returnUrl}?token={Uri.EscapeDataString(token)}");
            }


            [HttpPost("forgot-password")]
                public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
                {
                    if (!ModelState.IsValid)
                        return BadRequest(ModelState);

                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user == null)
                        return NotFound("User not found.");

                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetLink = $"{model.ClientAppUrl}/reset%20pass.html?email={Uri.EscapeDataString(model.Email)}&token={Uri.EscapeDataString(token)}";

                    var message = $@"
                    <h3>Reset Your Password</h3>
                    <p>Click the link below to reset your password:</p>
                    <a href='{resetLink}'>Reset Password</a>
                ";

                    await _emailSender.SendEmailAsync(model.Email, "Reset Your Password", message);

                    return Ok("Reset password link sent to your email.");
                }

                [HttpPost("reset-password")]
                public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
                {
                    if (!ModelState.IsValid)
                        return BadRequest(ModelState);

                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user == null)
                        return NotFound("User not found.");

                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                    if (result.Succeeded)
                        return Ok("Password reset successful.");


                    return BadRequest(result.Errors);
                }




            }
        }







