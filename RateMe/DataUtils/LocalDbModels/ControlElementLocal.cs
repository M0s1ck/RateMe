using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateMe.DataUtils.LocalDbModels
{
    public class ControlElementLocal
    {
        [Key]
        public int ElementId { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public double Grade { get; set; }

        public int SubjectId { get; set; }
        public SubjectLocal Subject { get; set; }
    }
}
