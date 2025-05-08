using RateMeApiServer.Models.Dto;

namespace RateMeApiServer.Services
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(int id);
        Task AddUserAsync(UserDto userResponseDto);
    }
}
