using RateMe.Models.JsonFileModels;
using RateMeShared.Dto;

namespace RateMe.Api.MainApi.Mappers;

public static class UserMapper
{
    internal static UserFullDto UserToFullDto(User user)
    {
        return new UserFullDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Surname = user.Surname,
            Password = user.Password,
            Curriculum = user.Curriculum,
            Year = user.Year,
            Quote = user.Quote,
        };
    }
    
    internal static User UserFromFullDto(UserFullDto dto)
    {
        return new User
        {
            Id = dto.Id,
            Email = dto.Email,
            Name = dto.Name,
            Surname = dto.Surname,
            Password = dto.Password,
            Curriculum = dto.Curriculum,
            Year = dto.Year,
            Quote = dto.Quote
        };
    }
}