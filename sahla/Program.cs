using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using sahla.DataAcess;
using sahla.Repositories;
using sahla.Repositories.Ireposotries;
using sahla.Utility;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Features;

namespace sahla
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add custom services
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://127.0.0.1:5500"
                        , "http://localhost:5500", "http://127.0.0.1:5501",
    "http://localhost:5501")
                          .AllowAnyHeader()
                          .AllowAnyMethod().
                          AllowCredentials();

                });
            });
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; 
            });
            static async Task SeedDatabase(WebApplication app)
            {
                using var scope = app.Services.CreateScope();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string adminname = "Admin";
                string adminEmail = "admin@admin.com";
                string adminPassword = "admin2002AaA@";
                string superAdminRole = "SuperAdmin";

                if (!await roleManager.RoleExistsAsync(superAdminRole))
                {
                    await roleManager.CreateAsync(new IdentityRole(superAdminRole));
                }

                // ✅ Check if user exists
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = adminname,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        Points = 1000000,
                    };

                    var result = await userManager.CreateAsync(user, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, superAdminRole);
                    }
                    else
                    {
                        // Logging or throw error
                        foreach (var error in result.Errors)
                            Console.WriteLine(error.Description);
                    }
                }
                else
                {
                    // Ensure user has the superadmin role
                    if (!await userManager.IsInRoleAsync(adminUser, superAdminRole))
                    {
                        await userManager.AddToRoleAsync(adminUser, superAdminRole);
                    }
                }
            }

            // Configure DB
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // ✅ JWT Authentication (Default)
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "https://localhost:7273",
                    ValidAudience = "http://127.0.0.1:5500", 
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("AbdullahosyHosnyHasona$abdullahamdy$hosnyhasona$abdullahosny$hamoda$y3amshla"))
                };
            })
            // ✅ Google External Login
            .AddGoogle("Google", googleOptions =>
            {
                googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                googleOptions.CallbackPath = "/signin-google"; 
                googleOptions.SaveTokens = true;
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();
            await SeedDatabase(app);



            // Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Middlewares
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    var path = ctx.File.Name;
                    if (path.EndsWith(".jpg") || path.EndsWith(".jpeg") || path.EndsWith(".png") || path.EndsWith(".gif"))
                    {
                        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://127.0.0.1:5500");
                        ctx.Context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
                        ctx.Context.Response.Headers.Append("Access-Control-Expose-Headers", "*");
                        ctx.Context.Response.Headers.Append("Content-Type", "image/jpeg");
                    }
                }
            });
            app.UseCors("AllowFrontend");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
