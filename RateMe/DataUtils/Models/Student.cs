
namespace RateMe.DataUtils.Models
{
    public class Student
    {
        public string Surname { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int Group { get; set; }
        
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
