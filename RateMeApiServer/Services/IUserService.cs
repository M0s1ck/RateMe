using RateMeApiServer.Common;
using RateMeShared.Dto;

namespace RateMeApiServer.Services
{
    public interface IUserService
    {
        Task<DbInteractionResult<UserDto>> GetUserByIdAsync(int id);
        Task<DbInteractionResult<int>> AddUserAsync(UserDto userDto);
        Task<DbInteractionResult<int>> AuthUserAsync(AuthRequest authRequestDto);
    }
}
