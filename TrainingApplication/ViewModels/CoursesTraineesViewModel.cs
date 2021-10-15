using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingApplication.Models;

namespace TrainingApplication.ViewModels
{
    public class CoursesTraineesViewModel
    {
        public Course Course { get; set; }
        public List<Trainee> Trainees { get; set; }
    }
}