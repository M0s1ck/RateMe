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
        public string Curriculum { get; }
        public int Course { get; }
        public int Semester { get; }


        public SyllabusModel(Student student, string curriculum, int course, int semester)
        {
            Student = student;
            Curriculum = curriculum;
            Course = course;
            Semester = semester;
        }

        public override string ToString()
        {
            return $"{Student.ToString()} {Curriculum} {Course} {Semester}";
        }
    }
}
