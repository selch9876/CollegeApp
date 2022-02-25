using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeApp.Models
{
    public class Subject
    {
 
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Subject Title")]
        public string? SubjectTitle { get; set; }
        public int CourseId { get; set; }



        //Relationships
        public virtual ICollection<Student>? Students { get; set; }

        public Course? Course { get; set; }
        public virtual Teacher? Teacher { get; set; }
        public virtual ICollection<Grade>? Grades { get; set; }


    }
}
