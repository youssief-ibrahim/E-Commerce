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
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDTO)
        {
            var result = await authenticationService.RegisterAsync(registerDTO);
            return HandleResult<UserDto>(result);
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
