using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateMe.DataUtils.Models;

namespace RateMe.DataUtils.LocalDbModels
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
            this.Subject = subj;
            Name = "NotInit";
        }
    }
}
