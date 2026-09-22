using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.DTOS.IDentityDTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace E_Commerce.Presentation.Controllers
{
    public class UserController : ApiBaseController
    {
        private readonly IUserService userService;
        public UserController(IUserService _userService)
        {
            userService = _userService;
        }
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordDto forgetPasswordDto)
        {
          var res=  await userService.ForgetPasswordAsync(forgetPasswordDto);
            return HandleResult(res);
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var res = await userService.ResetPasswordAsync(resetPasswordDto);
            return HandleResult(res);
        }
        [EnableRateLimiting("OtpVerifyPolicy")]
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto confirmEmailDto)
        {
            var res = await userService.ConfirmEmailAsync(confirmEmailDto);
            return HandleResult(res);
        }
        [EnableRateLimiting("OtpResendPolicy")]
        [HttpPost("ResendOtp")]
        public async Task<IActionResult> ResendOtp(string email)
        {
            var res = await userService.ResendOtpAsync(email);
            return HandleResult(res);
        }

    }
}
