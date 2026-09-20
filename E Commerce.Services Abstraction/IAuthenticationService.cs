using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOS.IDentityDTOS;

namespace E_Commerce.Services_Abstraction
{
    public interface IAuthenticationService
    {
        Task<Result<UserDto>> LoginAsync(LoginDto LoginDTO);
        Task<Result> RegisterAsync(RegisterDto RegisterDTO);
        Task<bool> CheckEmailAsync(string email);
        Task<Result<UserDto>> GetUserByEmailAsync(string email);
        Task<Result<UserDto>> RefreshTokenAsync(string refreshToken);
        Task<Result> RevokeTokenAsync(string refreshToken);
        Task<Result> RevokeAllTokensAsync(string userId); 
    }
}
