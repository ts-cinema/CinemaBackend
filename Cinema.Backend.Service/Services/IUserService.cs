using Cinema.Backend.Service.Models;
using Cinema.Backend.Service.Models.DTOs;

namespace Cinema.Backend.Service.Services
{
    public interface IUserService
    {
        Task<UserManagerResponse> RegisterUserAsync(UserRegistrationRequest request);

        Task<UserManagerResponse> LoginUserAsync(UserLoginRequest request);

        Task ChangeUserInfoAsync(UserChangeInformationRequest request, string loggedUserId);

        Task<List<User>> GetAllUsers();

        Task<UserProfileInfo> GetUserById(Guid id);
    }
}
