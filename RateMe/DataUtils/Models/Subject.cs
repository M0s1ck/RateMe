using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateMe.DataUtils.Models
{
    internal class Subject
    {
        internal string Name { get; }
        internal int Credits { get; }
        internal int[] Modules { get; }

        private Dictionary<string, string> _assFormulas;

        internal Subject(string name, int credits, int[] modules, Dictionary<string,string> assFormulas)
        {
            Name = name;
            Credits = credits;
            Modules = modules;
            _assFormulas = assFormulas;
        }

    }
}
