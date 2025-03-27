using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateMe.DataUtils.Models
{
    internal class Subject
    {
        internal string Name;
        internal int Credits;

        internal Subject(string name, /* formule, */ int credits)
        {
            Name = name;
            Credits = credits;
        }


    }
}
