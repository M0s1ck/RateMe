using System.ComponentModel.DataAnnotations;

namespace RateMeApiServer.Models.Entities;

public class ControlElement
{
    [Key]
    public int Id { get; set; }
    [MaxLength(200)]
    public required string Name { get; set; }
    public decimal Weight { get; set; }
    public decimal Grade { get; set; }

    public int SubjectId { get; set; }
    public Subject Subject { get; set; }
}