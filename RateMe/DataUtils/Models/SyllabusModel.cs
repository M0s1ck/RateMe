
namespace RateMe.DataUtils.Models
{
    public class SyllabusModel
    {
        public Student Student { get; set; }
        public string Curriculum { get; set; } = string.Empty;
        public int Course { get; set; }
        public int Module { get; set; }


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
