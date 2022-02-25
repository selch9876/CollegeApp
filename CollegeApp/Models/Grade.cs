using System.ComponentModel.DataAnnotations;

namespace CollegeApp.Models
{
    public class Grade
    {
        public int GradeId { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        [DisplayFormat(NullDisplayText = "No grade")]
        [Range(0, 100)]
        public int Value { get; set; }

        //Relationships
        public ICollection<Subject> subjects { get; set; }
        public ICollection<Student> students { get; set; }
    }
}
