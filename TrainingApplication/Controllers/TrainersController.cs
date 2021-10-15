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
    [Authorize(Roles = Role.Trainer)]
    public class TrainersController : Controller
    {
        private ApplicationDbContext _context;
        public TrainersController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Trainers
        [HttpGet]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var trainerInDb = _context.Trainers
                .SingleOrDefault(t => t.TrainerId == userId);
            return View(trainerInDb);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var trainerInDb = _context.Trainers
                .SingleOrDefault(t => t.Id == id);
            if (trainerInDb == null)
            {
                return HttpNotFound();
            }
            return View(trainerInDb);
        }

        [HttpPost]
        public ActionResult Edit(Trainer trainer)
        {
            var trainerInDb = _context.Trainers.SingleOrDefault(t => t.Id == trainer.Id);
            if (trainerInDb == null)
            {
                return HttpNotFound();
            }
            trainerInDb.Name = trainer.Name;
            trainerInDb.Age = trainer.Age;
            trainerInDb.Address = trainer.Address;
            trainerInDb.Specialty = trainer.Specialty;

            _context.SaveChanges();
            return RedirectToAction("index", "Trainers");
        }

        [HttpGet]
        public ActionResult Course()
        {
            var course = _context.Courses.ToList();
            var trainee = _context.Trainees.ToList();
            var userId = User.Identity.GetUserId();
            var trainerCourses = _context.TrainersCourses
                .Where(t => t.Trainer.TrainerId == userId)
                .Select(t => t.CourseId)
                .ToList();
            var traineeCourses = _context.TraineesCourses.ToList() ;
            foreach (var courseId in trainerCourses)
            {
                var trainees = _context.TraineesCourses
                .Where(t => t.CourseId == courseId)
                .ToList();
                traineeCourses.AddRange(trainees);
            }
            return View(traineeCourses);
        }
    }
}