using RateMeShared.Dto;

namespace RateMe.Models.JsonModels;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    
    public User() {}

    public User(UserDto dto)
    {
        Id = dto.Id;
        Email = dto.Email;
        Password = dto.Password;
        Name = dto.Name;
        Surname = dto.Surname;
    }
}