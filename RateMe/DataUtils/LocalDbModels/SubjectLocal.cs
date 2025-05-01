using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateMe.DataUtils.LocalDbModels
{
    public class SubjectLocal
    {
        [Key]
        public int SubjectId { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public List<ControlElementLocal> Elements { get; set; } = [];
    }
}
