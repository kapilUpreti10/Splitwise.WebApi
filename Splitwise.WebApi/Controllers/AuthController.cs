using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Splitwise.Contracts.DTOs.Auth;
using Splitwise.Services.Interfaces;

namespace Splitwise.WebApi.Controllers
{
    // Not under Admin/ or User/ areas on purpose: these are the only two
    // endpoints in the whole API that must work WITHOUT a token.
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var (succeeded, result, errors) = await _authService.RegisterAsync(dto);
            if (!succeeded) return BadRequest(errors);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var (succeeded, result, error) = await _authService.LoginAsync(dto);
            if (!succeeded) return Unauthorized(new { message = error });

            return Ok(result);
        }
    }
}
