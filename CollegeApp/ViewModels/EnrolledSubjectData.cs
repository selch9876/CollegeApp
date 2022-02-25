﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CollegeApp.ViewModels
{
    public class EnrolledSubjectData
    {
        public int SubjectID { get; set; }
        public string SubjectTitle { get; set; }
        public bool Enrolled { get; set; }
        public SelectList Subjects { set; get; }
        [DisplayFormat(NullDisplayText = "No grade")]
        [Range(0, 100)]
        public int Grade { get; set; }
    }
}
