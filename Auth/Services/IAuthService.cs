using Auth.Dto;
using Auth.Entities;

namespace Auth.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto userDto);

        Task<string?> LoginAsync(UserDto userDto);
    }
}
