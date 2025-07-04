using RateMeApiServer.Common;
using RateMeApiServer.Mappers;
using RateMeApiServer.Models.Entities;
using RateMeApiServer.Repositories;
using RateMeShared.Dto;

namespace RateMeApiServer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        
        public UserService(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }


        public async Task<DbInteractionResult<UserDto>> GetUserByIdAsync(int id)
        {
            DbInteractionResult<User> interaction = await _userRepository.GetByIdAsync(id);

            if (interaction.Status != DbInteractionStatus.Success)
            {
                return new DbInteractionResult<UserDto>(null, interaction.Status);
            }
            
            UserDto userDto = UserMapper.GetUserDto(interaction.Value!);
            return new DbInteractionResult<UserDto>(userDto, DbInteractionStatus.Success);
        }


        public async Task<DbInteractionResult<int>> AddUserAsync(UserDto userRequestDto)
        {
            User user = UserMapper.GetUserFromDto(userRequestDto);
            return await _userRepository.AddAsync(user);
        }

        
        public async Task<DbInteractionResult<UserDto>> AuthUserAsync(AuthRequest authRequest)
        { 
            DbInteractionResult<User> interaction = await _userRepository.AuthAsync(authRequest.Email, authRequest.Password);

            if (interaction.Status != DbInteractionStatus.Success)
            {
                return new DbInteractionResult<UserDto>(null, interaction.Status);
            }
            
            UserDto userDto = UserMapper.GetUserDto(interaction.Value!);
            return new DbInteractionResult<UserDto>(userDto, DbInteractionStatus.Success);
        }
    }
}
