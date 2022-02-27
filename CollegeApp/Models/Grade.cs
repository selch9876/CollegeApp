using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeApp.Models
{
    public class Grade
    {
        [Key]
        public int GradeId { get; set; }
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        [ForeignKey("Subject")]
        public int SubjectId { get; set; }
        [DisplayFormat(NullDisplayText = "No grade")]
        [Range(0, 100)]
        public int Value { get; set; }

        //Relationships
        public virtual Student Student { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
