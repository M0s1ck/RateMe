using System.ComponentModel.DataAnnotations;

namespace RateMe.Models.LocalDbModels
{
    public class ControlElementLocal
    {
        [Key]
        public int ElementId { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public decimal Weight { get; set; }
        public decimal Grade { get; set; }

        public int SubjectId { get; set; }
        public SubjectLocal Subject { get; set; }

        public ControlElementLocal()
        {
            Name = "NotInit";
        }

        public ControlElementLocal(SubjectLocal subj)
        {
            Subject = subj;
            Name = "NotInit";
        }
    }
}
