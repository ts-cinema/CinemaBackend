using Cinema.Backend.Service.Models.DTOs;
using Cinema.Backend.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Backend.Service.Controllers
{
    [Route("/api/v1/cinema/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegistrationRequest request)
        {
            UserManagerResponse response = await _userService.RegisterUserAsync(request);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequest request)
        {
            UserManagerResponse response = await _userService.LoginUserAsync(request);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
