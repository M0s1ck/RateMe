using RateMeShared.Dto;

namespace RateMe.Models.JsonFileModels;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Curriculum { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Quote { get; set; }= string.Empty;
    public bool IsDefaultPicture { get; set; } = true;
    public string PictureS3Id { get; set; } = string.Empty;
    public bool IsRemoteUpdated { get; set; } = true;
    
    public User() {}

    public User(UserDto dto) // TODO: update for cur and year
    {
        Id = dto.Id;
        Email = dto.Email;
        Password = dto.Password;
        Name = dto.Name;
        Surname = dto.Surname;
    }
}