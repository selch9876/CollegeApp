using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeApp.Models
{
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }
        [ForeignKey("Subject")]
        public int? SubjectID { get; set; }
        [Required]
        public string TeacherName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Birthday { get; set; }
        public int Salary { get; set; }

        


        //Relationships
        public virtual Subject? Subject { get; set; }
    }
}
