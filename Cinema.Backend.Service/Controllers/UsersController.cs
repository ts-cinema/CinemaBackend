using Cinema.Backend.Service.Models;
using Cinema.Backend.Service.Models.Core;
using Cinema.Backend.Service.Models.DTOs;
using Cinema.Backend.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Backend.Service.Controllers
{
    [Route("/api/v1/cinema/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly string _loggedUserId;

        public UsersController(IUserService usersService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = usersService;

            var userIdentifierClaim = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == Claims.USER_IDENTIFIER);

            if (userIdentifierClaim != null)
            {
                _loggedUserId = userIdentifierClaim.Value;
            }
        }

        [HttpPost("change-password")]
        [Authorize(Roles = Roles.REGISTERED_USER)]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordRequest request)
        {
            await _userService.ChangePasswordAsync(request, _loggedUserId);

            return Ok("Password was successfully changed.");
        }

        [HttpPost("change-username")]
        [Authorize(Roles = Roles.REGISTERED_USER)]
        public async Task<IActionResult> ChangeUserName([FromBody] UserChangeUserNameRequest request)
        {
            await _userService.ChangeUserNameAsync(request, _loggedUserId);

            return Ok("Username was successfully changed.");
        }

        [HttpGet]
        [Authorize(Roles = Roles.ADMINISTRATOR)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();

            return Ok(users);
        }
    }
}
