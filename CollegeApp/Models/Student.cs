using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeApp.Models
{
    public class Student
    {
        
        [Key]
        public int Id { get; set; }
        [Required]
        public string StudentName { get; set; }
        public DateTime Birthday { get; set; }
        
        public int? GradeId { get; set; }


        //Relationships
       
        public virtual ICollection<Grade>? Grades { get; set; }
        
        public ICollection<Subject>? Subjects { get; set; }
        
    }
}
