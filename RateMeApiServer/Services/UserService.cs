using RateMeApiServer.Common;
using RateMeApiServer.Mappers;
using RateMeApiServer.Models.Entities;
using RateMeApiServer.Repositories;
using RateMeShared.Dto;

namespace RateMeApiServer.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
        
    public UserService(IUserRepository userRepository) 
    {
        _userRepository = userRepository;
    }


    public async Task<DbInteractionResult<UserFullDto>> GetUserByIdAsync(int id)
    {
        DbInteractionResult<User> interaction = await _userRepository.GetByIdAsync(id);

        if (interaction.Status != DbInteractionStatus.Success)
        {
            return new DbInteractionResult<UserFullDto>(null, interaction.Status);
        }
            
        UserFullDto userDto = UserMapper.GetFullUserDto(interaction.Value!);
        return new DbInteractionResult<UserFullDto>(userDto, DbInteractionStatus.Success);
    }


    public async Task<DbInteractionResult<int>> AddUserAsync(UserDto userRequestDto)
    {
        User user = UserMapper.GetUserFromDto(userRequestDto);
        return await _userRepository.AddAsync(user);
    }

        
    public async Task<DbInteractionResult<UserFullDto>> AuthUserAsync(AuthRequest authRequest)
    { 
        DbInteractionResult<User> interaction = await _userRepository.AuthAsync(authRequest.Email, authRequest.Password);

        if (interaction.Status != DbInteractionStatus.Success)
        {
            return new DbInteractionResult<UserFullDto>(null, interaction.Status);
        }
            
        UserFullDto userDto = UserMapper.GetFullUserDto(interaction.Value!);
        return new DbInteractionResult<UserFullDto>(userDto, DbInteractionStatus.Success);
    }

    
    public async Task<DbInteractionStatus> UpdateAsync(UserFullDto userFullDto)
    {
        User user = UserMapper.GetFullUserFromDto(userFullDto);
        return await _userRepository.UpdateAsync(user);
    }
}