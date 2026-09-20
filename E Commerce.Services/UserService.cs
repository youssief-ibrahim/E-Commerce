using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.IdentityModule;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOS.IDentityDTOS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore.Storage;

namespace E_Commerce.Services
{
    public class UserService : IUserService
    {
        private readonly IEmailSender emailSender;
        private readonly ICacheRepository cacheRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public UserService(IEmailSender _emailSender, UserManager<ApplicationUser> _userManager, ICacheRepository _cacheRepository)
        {
            emailSender = _emailSender;
            userManager = _userManager;
            cacheRepository = _cacheRepository;
        }

        public async Task<Result> ConfirmEmailAsync(ConfirmEmailDto confirmDto)
        {
            var user = await userManager.FindByEmailAsync(confirmDto.Email);
            if (user == null) return Error.NotFound("User not found.");

            if (user.EmailConfirmed)
                return Result.Ok("Email is already confirmed.");

            var IsValidOpt= await VerifyOtpAsync(confirmDto);

            if(!IsValidOpt) return Error.Validation("Invalid Or Expired OTP.");

            user.EmailConfirmed = true;

            var result = await userManager.UpdateAsync(user);

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

        public async Task<string> GenerateOtpAsync(string email)
        {
            //var otp = new Random().Next(100000, 999999).ToString();
            var otp= RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            string kry= $"EmailOtp:{email}";
            await cacheRepository.SetAsync(
             kry,
             otp,
             TimeSpan.FromMinutes(2));

            return otp;
        }

        public async Task<Result> ResendOtpAsync(string email)
        {
            var user =await userManager.FindByEmailAsync(email);
            if (user == null)
                return Error.NotFound("User not found.");
            // 2. Check if email is already confirmed
            if (user.EmailConfirmed)
                return Error.Validation("Email is already confirmed.");

            var otp = await GenerateOtpAsync(user.Email!);

            var subject = "Confirm your email";

            var message =
                $"<p>Dear {user.UserName},</p>" +
                $"<p>Your new email confirmation code is:</p>" +
                $"<h1>{otp}</h1>" +
                $"<p>This code will expire in 2 minutes.</p>";

            
            await emailSender.SendEmailAsync(user.Email!,subject,message);

            return Result.Ok("A new OTP has been sent to your email.");
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

        private async Task<bool> VerifyOtpAsync(ConfirmEmailDto verifyOtpDto)
        {

            var key = $"EmailOtp:{verifyOtpDto.Email}";

            var storedOtp = await cacheRepository.GetAsync(key);

            if (storedOtp == null || storedOtp != verifyOtpDto.Opt)  return false;
            
            //await cacheRepository.DeleteAsync(key);

            return true;
        }
    }
}
