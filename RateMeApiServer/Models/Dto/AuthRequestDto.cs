namespace RateMeApiServer.Models.Dto;

public class AuthRequestDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}