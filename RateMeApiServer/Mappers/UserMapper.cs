using RateMeApiServer.Models.Entities;
using RateMeShared.Dto;

namespace RateMeApiServer.Mappers;

public static class UserMapper
{
    internal static UserDto GetUserDto(User user)
    {
        return new UserDto
        { 
            Id = user.Id,
            Email = user.Email,
            Password = user.Password,
            Name = user.Name,
            Surname = user.Surname
        };
    }

    internal static User GetUserFromDto(UserDto dto)
    {
        return new User
        {
            Email = dto.Email,
            Password = dto.Password,
            Name = dto.Name,
            Surname = dto.Surname
        };
    }
}