using System.ComponentModel.DataAnnotations;

namespace CollegeApp.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        //Relationships
        [Display(Name = "Subjects")]
        public ICollection<Subject>? subjects { get; set; }
    }
}
