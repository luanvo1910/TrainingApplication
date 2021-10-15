using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingApplication.Models;

namespace TrainingApplication.ViewModels
{
    public class TraineesCourseViewModel
    {
        public int CourseId { get; set; }
        public List<Course> Courses { get; set; }
        public int TraineeId { get; set; }
        public List<Trainee> Trainees { get; set; }
    }
}