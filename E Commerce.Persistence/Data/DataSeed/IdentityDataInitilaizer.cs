using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace E_Commerce.Persistence.Data.DataSeed
{
    public class IdentityDataInitilaizer : IDataInitializer
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<IdentityDataInitilaizer> logger;

        public IdentityDataInitilaizer(UserManager<ApplicationUser> _userManager,
                                        RoleManager<IdentityRole> _roleManager,
                                        ILogger<IdentityDataInitilaizer> _logger)
        {
            userManager = _userManager;
            roleManager = _roleManager;
            logger = _logger;
        }
        public async Task InitializeAsync()
        {
            try
            {
                if (!roleManager.Roles.Any())
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }
                if (!userManager.Users.Any())
                {
                    var user1 = new ApplicationUser()
                    {
                        Name="Admin",
                        UserName = "mrAdmin",
                        Email = "Admin@gmail.com",
                        PhoneNumber = "01234567890"
                    };
                    var user2 = new ApplicationUser()
                    {
                        Name = "PreUser",
                        UserName = "mrUser",
                        Email = "PreUser@gmail.com",
                        PhoneNumber = "01234567890"
                    };

                    var result1 = await userManager.CreateAsync(user1, "Admin@123");
                    if (result1.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user1, "Admin");
                    }
                    else
                    {
                        logger.LogError("Failed to create user1: {Errors}", string.Join(", ", result1.Errors.Select(e => e.Description)));
                    }

                    var result2 = await userManager.CreateAsync(user2, "User@123");
                    if (result2.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user2, "User");
                    }
                    else
                    {
                        logger.LogError("Failed to create user2: {Errors}", string.Join(", ", result2.Errors.Select(e => e.Description)));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while seeding identity data.={ex.Message}");
            }
        }
    }
}
