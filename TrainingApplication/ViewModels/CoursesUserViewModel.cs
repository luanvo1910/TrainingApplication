using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingApplication.Models;

namespace TrainingApplication.ViewModels
{
    public class CoursesUserViewModel
    {
        public Course Course { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}