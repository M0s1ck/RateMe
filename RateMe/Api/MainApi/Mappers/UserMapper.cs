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
}