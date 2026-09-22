using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.IdentityModule;
using E_Commerce.Persistence.Data.DataSeed;
using E_Commerce.Persistence.Data.DbContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace E_Commerce.Persistence.Data.DataExtensions
{
    public static class IdentityExtensions
    {
        public static int x;
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            // Data Initializers
            services.AddKeyedScoped<IDataInitializer, DataInitializer>("Default");

            services.AddKeyedScoped<IDataInitializer, IdentityDataInitilaizer>("Identity");

            // Identity
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<EcomerceDbContext>()
            .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(
                TokenOptions.DefaultProvider);

            return services;
        }

        public static IServiceCollection AddRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                // Resend OTP
                options.AddFixedWindowLimiter("OtpResendPolicy", opt =>
                {
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.PermitLimit = 5;
                    opt.QueueLimit = 0;
                });

                // Verify OTP
                options.AddFixedWindowLimiter("OtpVerifyPolicy", opt =>
                {
                    opt.Window = TimeSpan.FromMinutes(2);
                    opt.PermitLimit = 3;
                    opt.QueueLimit = 0;
                });

                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode =StatusCodes.Status429TooManyRequests;

                    context.HttpContext.Response.ContentType = "application/json";

                    var response = new
                    {
                        StatusCode = 429,
                        Message = $"Too many requests. Please try again later ."
                    };
                    await context.HttpContext.Response.WriteAsJsonAsync(response,cancellationToken);
                };
            });
            return services;
        }

    }
}
