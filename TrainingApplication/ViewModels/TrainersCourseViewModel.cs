using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingApplication.Models;

namespace TrainingApplication.ViewModels
{
    public class TrainersCourseViewModel
    {
        public int CourseId { get; set; }
        public List<Course> Courses { get; set; }
        public int TrainerId { get; set; }
        public List<Trainer> Trainers { get; set; }
    }
}