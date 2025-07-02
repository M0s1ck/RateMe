using System.ComponentModel.DataAnnotations;

namespace RateMe.Models.LocalDbModels
{
    public class SubjectLocal
    {
        [Key]
        public int SubjectId { get; set; }
        [MaxLength(200)]
        public required string Name { get; set; }
        public int Credits { get; set; }
        
        public int RemoteId { get; set; }
        public List<ElementLocal> Elements { get; set; } = [];
    }
}
