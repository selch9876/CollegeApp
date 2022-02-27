using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CollegeApp.ViewModels
{
    public class EnrolledSubjectData
    {
        public int SubjectID { get; set; }
        public string SubjectTitle { get; set; }
        public bool Enrolled { get; set; }
        public SelectList Subjects { set; get; }
        public int Grade { get; set; }
        public int GradeId { get; set; }
    }
}
