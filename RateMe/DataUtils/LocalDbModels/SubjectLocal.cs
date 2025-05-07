using System.ComponentModel.DataAnnotations;

namespace RateMe.DataUtils.LocalDbModels
{
    public class SubjectLocal
    {
        [Key]
        public int SubjectId { get; set; }
        public required string Name { get; set; }
        public int Credits { get; set; }
        public List<ControlElementLocal> Elements { get; set; } = [];
    }
}
