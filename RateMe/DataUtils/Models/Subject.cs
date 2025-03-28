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

        internal Subject(string name, int credits, Dictionary<string,string> assFormulas)
        {
            Name = name;
            Credits = credits;
        }


    }
}
