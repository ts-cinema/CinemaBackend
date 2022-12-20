using Cinema.Backend.Service.Models.DTOs;

namespace Cinema.Backend.Service.Services
{
    public interface IUserService
    {
        Task<UserManagerResponse> RegisterUserAsync(UserRegistrationRequest request);

        Task<UserManagerResponse> LoginUserAsync(UserLoginRequest request);

        Task ChangePasswordAsync(UserChangePasswordRequest request, string loggedUserId);

        Task ChangeUserNameAsync(UserChangeUserNameRequest request, string loggedUserId);
    }
}
