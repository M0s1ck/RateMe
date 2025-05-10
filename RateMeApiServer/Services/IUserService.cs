using RateMeApiServer.Models.Dto;

namespace RateMeApiServer.Services
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(int id);
        Task<int> AddUserAsync(UserDto userDto);
        Task<UserDto> AuthUserAsync(AuthRequestDto authRequestDto);
    }
}
