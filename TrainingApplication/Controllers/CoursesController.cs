using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TrainingApplication.Models;
using TrainingApplication.Utils;
using TrainingApplication.ViewModels;

namespace TrainingApplication.Controllers
{
    public class CoursesController : Controller
    {
        private ApplicationDbContext _context;
        public CoursesController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Courses
        [HttpGet]
        public ActionResult Index(string SearchCourse)
        {
            var courses = _context.Courses
                .Include(t => t.Category)
                .Include(t => t.Trainer)
                .ToList();
            if (!string.IsNullOrEmpty(SearchCourse))
            {
                courses = courses
                    .Where(t => t.Name.ToLower().Contains(SearchCourse.ToLower())).
                    ToList();
            }
            return View(courses);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var categories = _context.Categories.ToList();
            var trainers = _context.Trainers.ToList();
            var viewModel = new CoursesViewModel()
            {
                Categories = categories,
                Trainers = trainers
            };
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Create(CoursesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new CoursesViewModel
                {
                    Course = model.Course,
                    Categories = _context.Categories.ToList(),
                    Trainers = _context.Trainers.ToList()
                };
                return View(viewModel);
            }

            var courseTrainer = new CoursesTrainer()
            {
                CourseId = model.Course.Id,
                TrainerId = model.Course.TrainerId
            };
            var newCourse = new Course()
            {
                Name = model.Course.Name,
                Description = model.Course.Description,
                CategoryId = model.Course.CategoryId,
                TrainerId = model.Course.TrainerId
            };
            _context.Courses.Add(newCourse);
            _context.CoursesTrainer.Add(courseTrainer);
            _context.SaveChanges();
            return RedirectToAction("Index", "Courses");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var CourseInDb = _context.Courses
                .SingleOrDefault(t => t.Id == id);
            var CoursesTrainerInDb = _context.CoursesTrainer
                .SingleOrDefault(t => t.CourseId == id);
            if (CourseInDb == null)
            {
                return HttpNotFound();
            }
            _context.Courses.Remove(CourseInDb);
            _context.CoursesTrainer.Remove(CoursesTrainerInDb);
            _context.SaveChanges();
            return RedirectToAction("Index", "Courses");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var CourseInDb = _context.Courses
                .SingleOrDefault(t => t.Id == id);
            if (CourseInDb == null)
            {
                return HttpNotFound();
            }
            var viewModel = new CoursesViewModel
            {
                Course = CourseInDb,
                Categories = _context.Categories.ToList(),
                Trainers = _context.Trainers.ToList()
            };
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Edit(CoursesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new CoursesViewModel
                {
                    Course = model.Course,
                    Categories = _context.Categories.ToList(),
                    Trainers = _context.Trainers.ToList()
                };
                return View(viewModel);
            }
            var CourseInDb = _context.Courses
                .SingleOrDefault(t => t.Id == model.Course.Id);
            if (CourseInDb == null)
            {
                return HttpNotFound();
            }
            CourseInDb.Name = model.Course.Name;
            CourseInDb.Description = model.Course.Description;
            CourseInDb.CategoryId = model.Course.CategoryId;
            CourseInDb.TrainerId = model.Course.TrainerId;

            var CoursesTrainerInDb = _context.CoursesTrainer
                .SingleOrDefault(t => t.CourseId == model.Course.Id);
            CoursesTrainerInDb.TrainerId = model.Course.TrainerId;
            _context.SaveChanges();
            return RedirectToAction("Index", "Courses");
        }

    }
}