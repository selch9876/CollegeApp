using System.Collections.Generic;
using CollegeApp.Models;

namespace CollegeApp.ViewModels
{
    public class StudentIndexData
    {
        public IEnumerable<Student> Students { get; set; }
        public IEnumerable<Subject> Subjects { get; set; }
        public IEnumerable<Grade> Grades { get; set; }
    }
}