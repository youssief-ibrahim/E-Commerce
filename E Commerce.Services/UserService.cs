using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using E_Commerce.Domain.Entities.IdentityModule;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOS.IDentityDTOS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace E_Commerce.Services
{
    public class UserService : IUserService
    {
        private readonly IEmailSender emailSender;
        private readonly UserManager<ApplicationUser> userManager;

        public UserService(IEmailSender _emailSender, UserManager<ApplicationUser> _userManager)
        {
            emailSender = _emailSender;
            userManager = _userManager;
        }

        public async Task<Result> ConfirmEmalAsync(ConfirmEmailDto confirmDto)
        {
            var user = await userManager.FindByEmailAsync(confirmDto.Email);
            if (user == null) return Error.NotFound("User not found.");

            var decodedToken = Uri.UnescapeDataString(confirmDto.Token);

            var result = await userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
                return errors;
            }
            return Result.Ok("Email confirmed successfully.");
        }

        public async Task<Result> ForgetPasswordAsync(ForgetPasswordDto forgetPasswordDto)
        {
           
            var webLink = forgetPasswordDto.WebLink;
            if (webLink == null) 
                return Error.Validation("WebLink is required for password reset.");
            
            var user= await userManager.FindByEmailAsync(forgetPasswordDto.Email);
            if (user == null) 
                return Error.NotFound($"User Not Found with this  Mail {forgetPasswordDto.Email}");
            
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResetLink = $"{webLink}?email={forgetPasswordDto.Email}&token={Uri.EscapeDataString(token)}";

            var subject = "Reset Password";

            var message = $"<p>Dear {user.UserName},</p>" +
                          $"<p>You requested to reset your password. Please click the link below to reset your password:</p>" +
                          $"<p><a href='{passwordResetLink}'>Reset Password</a></p>";

            await  emailSender.SendEmailAsync(forgetPasswordDto.Email!, subject, message);
            
            return Result.Ok($"Password reset link has been sent to {forgetPasswordDto.Email}");
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null) return Error.NotFound("User not found.");

            var decodedToken = Uri.UnescapeDataString(resetPasswordDto.Token);

            var result = await userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
                return errors;
            }
            
            return Result.Ok("Password has been reset successfully.");
        }

        public async Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            var user =await userManager.FindByEmailAsync(verifyOtpDto.Email);
            if (user == null) return false;

            var isValid = await userManager.VerifyUserTokenAsync(
                user,
                userManager.Options.Tokens.EmailConfirmationTokenProvider,
                UserManager<ApplicationUser>.ConfirmEmailTokenPurpose, 
                verifyOtpDto.Otp
                );
            if (!isValid) return false;

            return true;
        }
    }
}
