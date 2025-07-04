using System.ComponentModel.DataAnnotations;

namespace RateMeApiServer.Models.Entities;

public class Subject
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(200)]
    public required string Name { get; set; }
    public int Credits { get; set; }
    
    public int  UserId { get; set; }
    public User User { get; set; }
    
    public List<Element> Elements { get; set; } = [];
}