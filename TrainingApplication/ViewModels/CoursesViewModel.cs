﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingApplication.Models;

namespace TrainingApplication.ViewModels
{
    public class CoursesViewModel
    {
        public Course Course { get; set; }
        public List<Category> Categories { get; set; }
        public List<Trainer> Trainers { get; set; }
    }
}