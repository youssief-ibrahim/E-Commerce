using E_Commerce.Domain.Entities.IdentityModule;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace E_Commerce.Web.Extentions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,IConfiguration configuration)
        {
            var jwtSecret = configuration["Jwt:SecretKey"];

            if (string.IsNullOrWhiteSpace(jwtSecret))
            {
                throw new InvalidOperationException(
                    "JWT SecretKey is missing from configuration.");
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = false;
                options.RequireHttpsMetadata = true;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,

                        ClockSkew = TimeSpan.Zero,

                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSecret))
                    };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userManager = context.HttpContext.RequestServices
                                   .GetRequiredService<UserManager<ApplicationUser>>();

                        var userId = context.Principal?
                            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                        if (userId is null)
                        {
                            context.Fail(
                                "Unauthorized: missing user identifier.");
                            return;
                        }

                        var tokenVersionClaim = context.Principal?
                            .FindFirst("TokenVersion")?.Value;

                        if (tokenVersionClaim is null ||!int.TryParse(tokenVersionClaim, out var tokenVersion))
                        {
                            context.Fail(
                                "Unauthorized: missing or invalid token version.");
                            return;
                        }

                        var user = await userManager.FindByIdAsync(userId);

                        if (user is null ||
                            user.TokenVersion != tokenVersion)
                        {
                            context.Fail(
                                "Unauthorized: token has been invalidated.");
                        }
                    }
                };
            });

            return services;
        }
    }
}
