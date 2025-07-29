using RateMeApiServer.Common;
using RateMeShared.Dto;

namespace RateMeApiServer.Services;

public interface IUserService
{
    Task<DbInteractionResult<UserFullDto>> GetUserByIdAsync(int id);
    Task<DbInteractionResult<int>> AddUserAsync(UserDto userDto);
    Task<DbInteractionResult<UserFullDto>> AuthUserAsync(AuthRequest authRequestDto);
    Task<DbInteractionStatus> UpdateAsync(UserFullDto userFullDto);
}