using BookcaseAPI.DTOs;
using BookcaseAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookcaseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var response = await _authService.Register(request);
            if (response == null)
            {
                return BadRequest(new { message = "Username already exists" });
            }

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.Login(request);
            if (response == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            return Ok(response);
        }
    }
}