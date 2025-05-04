using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateMe.DataUtils.Models
{
    public class Student
    {
        public string Surname { get; } = string.Empty;

        public string Name { get; } = string.Empty;

        public int Group { get; }
        
        public Student() {}
        
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
