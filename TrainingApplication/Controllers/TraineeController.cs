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
        public ActionResult Courses()
        {
            var userId = User.Identity.GetUserId();

            var courses = _context.TraineesCourses
                .Where(t => t.Trainee.TraineeId == userId)
                .Select(t => t.Course)
                .ToList();
            return View(courses);
        }

        [HttpGet]
        public ActionResult CourseTrainees(int id)
        {
            var traineesCourse = _context.TraineesCourses
                .Where(t => t.CourseId == id)
                .Select(t => t.Trainee)
                .ToList();
            return View(traineesCourse);
        }
    }
}
