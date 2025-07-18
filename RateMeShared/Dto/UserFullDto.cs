using RateMeShared.Enums;

namespace RateMeShared.Dto;

public class UserFullDto : UserDto
{
    public string Curriculum { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Quote { get; set; } = string.Empty;
}