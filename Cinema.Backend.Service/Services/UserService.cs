using AspNetCore.Identity.MongoDbCore.Models;
using Cinema.Backend.Service.Models;
using Cinema.Backend.Service.Models.Core;
using Cinema.Backend.Service.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using Template.Service.Configuration;

namespace Cinema.Backend.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public UserService(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task ChangePasswordAsync(UserChangePasswordRequest request, string loggedUserId)
        {
            User user = await _userManager.FindByIdAsync(loggedUserId);

            IdentityResult result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                IList<string> errors = result.Errors.Select(e => e.Description).ToList();
                string errorMessage = string.Join('\n', errors);
                throw new ArgumentException(errorMessage);
            }
        }

        public async Task ChangeUserNameAsync(UserChangeUserNameRequest request, string loggedUserId)
        {
            User user = await _userManager.FindByIdAsync(loggedUserId);

            IdentityResult result = await _userManager.SetUserNameAsync(user, request.UserName);

            if (!result.Succeeded)
            {
                IList<string> errors = result.Errors.Select(e => e.Description).ToList();
                string errorMessage = string.Join('\n', errors);
                throw new ArgumentException(errorMessage);
            }
        }

        public async Task<UserManagerResponse> LoginUserAsync(UserLoginRequest request)
        {
            User user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                throw new AuthenticationException("Invalid login data");
            }

            bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isPasswordCorrect)
            {
                throw new AuthenticationException("Invalid login data");
            }

            var token = await GenerateTokenAsync(user);

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new UserManagerResponse
            {
                Message = "Successful login attempt",
                AccessToken = tokenAsString,
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };
        }

        public async Task<UserManagerResponse> RegisterUserAsync(UserRegistrationRequest request)
        {
            if (request.Password != request.ConfirmPassword)
            {
                throw new ArgumentException("Confirmed password doesn't match original password");
            }

            User user = new User
            {
                Email = request.Email,
                UserName = request.Email,
                Name = request.Name,
                BirthDate = request.BirthDate,
                Address = request.Address
            };

            IdentityResult result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.REGISTERED_USER);

                return new UserManagerResponse
                {
                    Message = $"User {user.Name} was successfully registered with email: {user.Email}",
                    IsSuccess = true
                };
            }
            else
            {
                return new UserManagerResponse
                {
                    Message = "User registration attempt failed",
                    IsSuccess = false,
                    Errors = result.Errors.Select(e => e.Description)
                };
            }
        }

        private async Task<JwtSecurityToken> GenerateTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(Claims.EMAIL_ADDRESS, user.Email),
                new Claim(Claims.USER_IDENTIFIER, user.Id.ToString()),
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(Claims.USER_ROLE, role));
            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetTokenSigningKey()));

            JwtSecurityToken token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha512)
            );

            return token;
        }
    }
}
