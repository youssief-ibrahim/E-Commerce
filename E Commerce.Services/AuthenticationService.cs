using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.IdentityModule;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOS.IDentityDTOS;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace E_Commerce.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration config;
        private readonly IEmailSender emailSender;
        private readonly IUserService userService;
        private readonly IRefreshTokenRepository refreshTokenRepository;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
           IEmailSender _emailSender,
            IConfiguration config,
            IRefreshTokenRepository refreshTokenRepository,
            IUserService _userService)
        {
            this.userManager = userManager;
            emailSender = _emailSender;
            this.config = config;
            this.refreshTokenRepository = refreshTokenRepository;
            userService = _userService;
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
            return new UserDto(user.Email!, user.Name, await GenerateAccessTokenAsync(user));
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
            if(!await userManager.IsEmailConfirmedAsync(user))
            {
                return Error.Validation("Email not confirmed. Please check your email for confirmation instructions.");
            }
            return await GenerateAuthResultAsync(user);
        }

        public async Task<Result> RegisterAsync(RegisterDto RegisterDTO)
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
                await userManager.AddToRoleAsync(user, "User");
                var token=await userManager.GenerateEmailConfirmationTokenAsync(user);

                //var confirmationLink = $"{RegisterDTO.WebLink}?email={user.Email}&token={Uri.EscapeDataString(token)}";

               var otp = await userService.GenerateOtpAsync(user.Email!);

                var subject = "Confirm your email";
                var message = $"<p>Dear {user.UserName},</p>" +
                              $"<p>Your email confirmation code is:</p>" +
                              $"<h1>{otp}</h1>" +
                              $"<p>This code will expire in 2 minutes.</p>";

                await emailSender.SendEmailAsync(RegisterDTO.email, subject, message);

                return Result.Ok("Registration successful. Please check your email to confirm your account.");
            }
            return IdentityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
        }

        public async Task<Result<UserDto>> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return Error.Validation("Refresh token is required");

            var storedToken = await refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (storedToken is null)
                return Error.Unauthorized("Invalid refresh token", "The refresh token is invalid");

            if (storedToken.IsExpired)
                return Error.Unauthorized("Invalid refresh token", "The refresh token is expired");

            if (storedToken.RevokedOn is not null)
                return await HandleRefreshTokenReuseAsync(storedToken.UserId);

            var user = await userManager.FindByIdAsync(storedToken.UserId);
            if (user is null)
                return Error.NotFound("User Not Found");

            user.TokenVersion++;

            var (newRefreshToken, plainToken) = CreateRefreshToken(user.Id);
            storedToken.RevokedOn = DateTime.Now;
            storedToken.ReplacedByToken = newRefreshToken.Token;

            refreshTokenRepository.Update(storedToken);
            await refreshTokenRepository.AddAsync(newRefreshToken);

            var updateResult = await userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return Error.Failure("User update failed");

            await refreshTokenRepository.SaveChangesAsync();

            var accessToken = await GenerateAccessTokenAsync(user);
            return new UserDto(user.Email!, user.Name, accessToken, plainToken, newRefreshToken.ExpiresOn);
        }

        public async Task<Result> RevokeTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return Error.Validation("Refresh token is required");

            var storedToken = await refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (storedToken is null || !storedToken.IsActive)
                return Error.Unauthorized("Invalid refresh token", "The refresh token is invalid or already revoked");

            storedToken.RevokedOn = DateTime.Now;
            refreshTokenRepository.Update(storedToken);
            await refreshTokenRepository.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<Result> RevokeAllTokensAsync(string userId)
        {
            await refreshTokenRepository.RevokeAllActiveForUserAsync(userId);
            await refreshTokenRepository.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<Result> LogoutAsync(string userId, string refreshToken)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return Error.NotFound("User Not Found");

            // جيب آخر refresh token صالح للـ user تلقائياً
            var latestActiveToken = await refreshTokenRepository.GetLatestActiveByUserIdAsync(userId);

            // لو فيه token صالح → اعمله revoke
            if (latestActiveToken is not null)
            {
                latestActiveToken.RevokedOn = DateTime.Now;
                refreshTokenRepository.Update(latestActiveToken);
            }

            // Increment TokenVersion → invalidates the current access token immediately
            user.TokenVersion++;
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return Error.Failure("Failed to invalidate session");

            await refreshTokenRepository.SaveChangesAsync();
            return Result.Ok("Logged out successfully");
        }

        private async Task<UserDto> GenerateAuthResultAsync(ApplicationUser user)
        {
            var accessToken = await GenerateAccessTokenAsync(user);
            var (refreshToken, plainToken) = CreateRefreshToken(user.Id);
            await refreshTokenRepository.AddAsync(refreshToken);
            await refreshTokenRepository.SaveChangesAsync();
            return new UserDto(user.Email!, user.Name, accessToken, plainToken, refreshToken.ExpiresOn);
        }

        private (RefreshToken Token, string PlainToken) CreateRefreshToken(string userId)
        {
            var plainToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var token = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = plainToken,
                ExpiresOn = DateTime.Now.AddDays(GetRefreshTokenExpirationDays()),
                CreatedOn = DateTime.Now,
                UserId = userId
            };
            return (token, plainToken);
        }

        private async Task<Result<UserDto>> HandleRefreshTokenReuseAsync(string userId)
        {
            await refreshTokenRepository.RevokeAllActiveForUserAsync(userId);
            await refreshTokenRepository.SaveChangesAsync();
            return Error.Unauthorized("Refresh token reuse detected", "This refresh token is no longer valid. Please login again");
        }

     

        private async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Name, user.Name!),
                new Claim("TokenVersion", user.TokenVersion.ToString())
            };
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = config["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT SecretKey is not configured in user secrets or appsettings.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(GetAccessTokenExpirationMinutes()),
                signingCredentials: creds,
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"]
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private int GetAccessTokenExpirationMinutes()
        {
            var value = config["Jwt:AccessTokenExpirationMinutes"];
            return int.TryParse(value, out var minutes) && minutes > 0 ? minutes : 15;  
        }

        private int GetRefreshTokenExpirationDays()
        {
            var value = config["Jwt:RefreshTokenExpirationDays"];
            return int.TryParse(value, out var days) && days > 0 ? days : 14;   
        }

    }
}
