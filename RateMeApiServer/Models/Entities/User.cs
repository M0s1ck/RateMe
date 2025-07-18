using System.ComponentModel.DataAnnotations;

namespace RateMeApiServer.Models.Entities;

public class User
{
    [Key]
    public int Id { get; set; }
    [MaxLength(200)]
    public required string Email { get; set; }
    [MaxLength(200)]
    public required string Password { get; set; }
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(200)]
    public string Surname { get; set; } = string.Empty;
    [MaxLength(200)]
    public string Curriculum { get; set; } = string.Empty;
    public int Year { get; set; }
    [MaxLength(500)]
    public string Quote { get; set; }= string.Empty;

    public List<Subject> Subjects { get; set; } = [];
}