using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateMe.DataUtils.Models
{
    public class Student
    {
        public string Surname { get; }

        public string Name { get; }

        public int Group { get; }


        public Student(string surname, string name, int group)
        {
            Surname = surname;
            Name = name;
            Group = group;
        }

        public override string ToString()
        {
            return $"{Surname} {Name} : {Group}";
        }


    }
}
