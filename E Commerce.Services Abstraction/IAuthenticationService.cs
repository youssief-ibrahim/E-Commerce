using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOS.IDentityDTOS;

namespace E_Commerce.Services_Abstraction
{
    public interface IAuthenticationService
    {
        Task<Result<UserDto>> LoginAsync(LoginDto LoginDTO);
        Task<Result<UserDto>> RegisterAsync(RegisterDto RegisterDTO);
        Task<bool> CheckEmailAsync(string email);
        Task<Result<UserDto>> GetUserByEmailAsync(string email);
    }
}
