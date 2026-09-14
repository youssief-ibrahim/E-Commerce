using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using E_Commerce.Domain.Entities.IdentityModule;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOS.IDentityDTOS;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace E_Commerce.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration config;
        private readonly IMapper mapper;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IConfiguration config, IMapper mapper)
        {
            this.userManager = userManager;
            this.config = config;
            this.mapper = mapper;
        }
        public async Task<bool> CheckEmailAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<Result<UserDto>> GetUserByEmailAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return Error.NotFound("User Not Found");
            }
            return new UserDto(user.Email!, user.Name, await GenerateTokenAsync(user));
        }
        public async Task<Result<UserDto>> LoginAsync(LoginDto LoginDTO)
        {
            var user = await userManager.FindByEmailAsync(LoginDTO.email);
            if (user == null)
            {
                return Error.InvalidCrendentials("User.InvalidCrendentials");
            }
            var IsPasswordValid = await userManager.CheckPasswordAsync(user, LoginDTO.password);
            if (!IsPasswordValid)
            {
                return Error.InvalidCrendentials("User.InvalidCrendentials");
            }
            var token = await GenerateTokenAsync(user);
            return new UserDto(user.Email!, user.Name, token);
        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterDto RegisterDTO)
        {
            var user = new ApplicationUser
            {
                Email = RegisterDTO.email,
                Name = RegisterDTO.Name,
                UserName = RegisterDTO.UserName,
                PhoneNumber = RegisterDTO.PhoneNumber
            };
            var IdentityResult = await userManager.CreateAsync(user, RegisterDTO.password);
            if (IdentityResult.Succeeded)
            {
                var token = await GenerateTokenAsync(user);
                return new UserDto(user.Email!, user.Name, token);
            }
            return IdentityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
        }

        private async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Name, user.Name!)
            };
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = config["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT SecretKey is not configured in appsettings.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds,
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"]
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
