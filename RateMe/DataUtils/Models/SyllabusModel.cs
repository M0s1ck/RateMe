using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateMe.DataUtils.Models
{
    public class SyllabusModel
    {
        public Student Student { get; }
        public string Curriculum { get; } = string.Empty;
        public int Course { get; }
        public int Module { get; }


        public SyllabusModel()
        {
            Student = new Student();
        }
        
        public SyllabusModel(Student student, string curriculum, int course, int semester)
        {
            Student = student;
            Curriculum = curriculum;
            Course = course;
            Module = semester;
        }

        public override string ToString()
        {
            return $"{Student} {Curriculum} {Course} {Module}";
        }
    }
}
