using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOS.IDentityDTOS;

namespace E_Commerce.Services_Abstraction
{
    public interface IUserService
    {
        Task<Result> ForgetPasswordAsync(ForgetPasswordDto forgetPasswordDto);
        Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<string> GenerateOtpAsync(string email);
        Task<Result> ConfirmEmailAsync(ConfirmEmailDto confirmDto);
        Task<Result> ResendOtpAsync(string email);

    }
}
