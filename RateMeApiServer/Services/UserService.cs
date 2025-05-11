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


        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            User? user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={id} was not found");
            }

            UserDto userDto = new()
            { 
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                Name = user.Name,
                Surname = user.Surname
            };

            return userDto;
        }


        public async Task<int> AddUserAsync(UserDto userRequestDto)
        {
            User user = new()
            {
                Email = userRequestDto.Email,
                Password = userRequestDto.Password,
                Name = userRequestDto.Name,
                Surname = userRequestDto.Surname
            };

            return await _userRepository.AddAsync(user);
        }

        public async Task<UserDto> AuthUserAsync(AuthRequest authRequest)
        {
            User user = await _userRepository.AuthAsync(authRequest.Email, authRequest.Password);
            UserDto userDto = new()
            { 
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                Name = user.Name,
                Surname = user.Surname
            };

            return userDto;
        }
    }
}
