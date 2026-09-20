using System.Security.Claims;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.DTOS.IDentityDTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Presentation.Controllers
{
    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationService authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDTO)
        {
            var result = await authenticationService.LoginAsync(loginDTO);
            return HandleResult<UserDto>(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDTO)
        {
            var result = await authenticationService.RegisterAsync(registerDTO);
            return HandleResult(result);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<UserDto>> Refresh(RefreshTokenRequestDto request)
        {
            var result = await authenticationService.RefreshTokenAsync(request.RefreshToken);
            return HandleResult(result);
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke(RefreshTokenRequestDto request)
        {
            var result = await authenticationService.RevokeTokenAsync(request.RefreshToken);
            return HandleResult(result);
        }

        [Authorize]
        [HttpPost("revoke-all")]
        public async Task<IActionResult> RevokeAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await authenticationService.RevokeAllTokensAsync(userId!);
            return HandleResult(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await authenticationService.LogoutAsync(userId!, string.Empty);
            return HandleResult(result);
        }

        [HttpGet("check-email")]
        public async Task<ActionResult<bool>> CheckEmail(string email)
        {
            var result = await authenticationService.CheckEmailAsync(email);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-user-by-email")]
        public async Task<ActionResult<UserDto>> GetUserByEmail()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await authenticationService.GetUserByEmailAsync(email!);
            return HandleResult(result);
        }

    }
}
