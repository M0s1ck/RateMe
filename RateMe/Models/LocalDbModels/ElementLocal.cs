using System.ComponentModel.DataAnnotations;
using RateMe.Repositories;

namespace RateMe.Models.LocalDbModels;

public class ElementLocal
{
    [Key]
    public int ElementId { get; set; }
    [MaxLength(200)]
    public required string Name { get; set; }
    public decimal Weight { get; set; }
    public decimal Grade { get; set; }

    public int SubjectId { get; set; }
    public SubjectLocal? Subject { get; set; }
        
    public int RemoteId { get; set; }
    public RemoteStatus RemoteStatus { get; set; }
}