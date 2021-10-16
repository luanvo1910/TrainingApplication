using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrainingApplication.Models;
using TrainingApplication.Utils;
using TrainingApplication.ViewModels;

namespace TrainingApplication.Controllers
{
    [Authorize(Roles = Role.Trainee)]
    public class TraineeController : Controller
    {
        private ApplicationDbContext _context;
        public TraineeController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Trainee
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var traineeInDb = _context.Trainees
                .SingleOrDefault(t => t.TraineeId == userId);
            return View(traineeInDb);
        }

        [HttpGet]
        public ActionResult Course()
        {
            var userId = User.Identity.GetUserId();

            var courseIds = _context.TraineesCourses
                .Where(t => t.Trainee.TraineeId == userId)
                .Select(t => t.CourseId)
                .ToList();

            List<CoursesTraineesViewModel> traineeCourses = new List<CoursesTraineesViewModel>();

            foreach (var courseId in courseIds)
            {
                var trainees = _context.TraineesCourses
                .Where(t => t.CourseId == courseId)
                .GroupBy(i => i.Course)
                .Select(res => new CoursesTraineesViewModel
                {
                    Course = res.Key,
                    Trainees = res.Select(u => u.Trainee).ToList()
                })
                .ToList();
                traineeCourses.AddRange(trainees);
            }

            return View(traineeCourses);
        }
    }
}