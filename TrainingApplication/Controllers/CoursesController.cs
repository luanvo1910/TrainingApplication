﻿using Microsoft.AspNet.Identity;
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
    [Authorize(Roles = Role.Staff)]
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
                .ToList();
            var trainer = _context.TrainersCourses.ToList();

            List<CoursesTrainerViewModel> viewModel = _context.TrainersCourses
                .GroupBy(i => i.Course)
                .Select(res => new CoursesTrainerViewModel
                {
                    Course = res.Key,
                    Trainers = res.Select(u => u.Trainer).ToList()
                })
                .ToList();

            if (!string.IsNullOrEmpty(SearchCourse))
            {
                viewModel = viewModel
                    .Where(t => t.Course.Name.ToLower().Contains(SearchCourse.ToLower())).
                    ToList();
            }
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult GetTrainees(string SearchCourse)
        {
            var courses = _context.Courses
                .Include(t => t.Category)
                .ToList();
            var trainee = _context.TraineesCourses.ToList();

            List<CoursesTraineesViewModel> viewModel = _context.TraineesCourses
                .GroupBy(i => i.Course)
                .Select(res => new CoursesTraineesViewModel
                {
                    Course = res.Key,
                    Trainees = res.Select(u => u.Trainee).ToList()
                })
                .ToList();
            if (!string.IsNullOrEmpty(SearchCourse))
            {
                viewModel = viewModel
                    .Where(t => t.Course.Name.ToLower().Contains(SearchCourse.ToLower())).
                    ToList();
            }
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var categories = _context.Categories.ToList();
            var viewModel = new CoursesViewModel()
            {
                Categories = categories,
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
                    Categories = _context.Categories.ToList()
                };
                return View(viewModel);
            }

            var newCourse = new Course()
            {
                Name = model.Course.Name,
                Description = model.Course.Description,
                CategoryId = model.Course.CategoryId,
            };
            _context.Courses.Add(newCourse);
            _context.SaveChanges();
            return RedirectToAction("Index", "Courses");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var CourseInDb = _context.Courses
                .SingleOrDefault(t => t.Id == id);
            var CoursesTraineeInDb = _context.TraineesCourses
                .SingleOrDefault(t => t.CourseId == id);
            var CoursesTrainerInDb = _context.TrainersCourses
                .SingleOrDefault(t => t.CourseId == id);
            if (CourseInDb == null)
            {
                return HttpNotFound();
            }
            _context.TrainersCourses.Remove(CoursesTrainerInDb);
            _context.TraineesCourses.Remove(CoursesTraineeInDb);
            _context.Courses.Remove(CourseInDb);
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
                Categories = _context.Categories.ToList()
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
                    Categories = _context.Categories.ToList()
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
            _context.SaveChanges();
            return RedirectToAction("Index", "Courses");
        }

        [HttpGet]
        public ActionResult AddTrainee()
        {
            var viewModel = new TraineesCourseViewModel
            {
                Courses = _context.Courses.ToList(),
                Trainees = _context.Trainees.ToList()
            };
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult AddTrainee(TraineesCourseViewModel viewModel)
        {
            var model = new TraineesCourse
            {
                CourseId = viewModel.CourseId,
                TraineeId = viewModel.TraineeId
            };
            _context.TraineesCourses.Add(model);
            _context.SaveChanges();

            return RedirectToAction("GetTrainees", "Courses");
        }

        [HttpGet]
        public ActionResult RemoveTrainee()
        {
            var trainees = _context.TraineesCourses.Select(t => t.Trainee)
                .Distinct()
                .ToList();
            var courses = _context.TraineesCourses.Select(t => t.Course)
                .Distinct()
                .ToList();

            var viewModel = new TraineesCourseViewModel
            {
                Courses = courses,
                Trainees = trainees
            };
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult RemoveTrainee(TraineesCourseViewModel viewModel)
        {
            var userTeam = _context.TraineesCourses
                .SingleOrDefault(t => t.CourseId == viewModel.CourseId && t.TraineeId == viewModel.TraineeId);
            if (userTeam == null)
            {
                return HttpNotFound();
            }

            _context.TraineesCourses.Remove(userTeam);
            _context.SaveChanges();

            return RedirectToAction("GetTrainees", "Courses");
        }

        [HttpGet]
        public ActionResult AddTrainer()
        {
            var viewModel = new TrainersCourseViewModel
            {
                Courses = _context.Courses.ToList(),
                Trainers = _context.Trainers.ToList()
            };
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult AddTrainer(TrainersCourseViewModel viewModel)
        {
            var model = new TrainersCourse
            {
                CourseId = viewModel.CourseId,
                TrainerId = viewModel.TrainerId
            };
            _context.TrainersCourses.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index", "Courses");
        }

        [HttpGet]
        public ActionResult RemoveTrainer()
        {
            var trainers = _context.TrainersCourses.Select(t => t.Trainer)
                .Distinct()
                .ToList();
            var courses = _context.TrainersCourses.Select(t => t.Course)
                .Distinct()
                .ToList();

            var viewModel = new TrainersCourseViewModel
            {
                Courses = courses,
                Trainers = trainers
            };
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult RemoveTrainer(TrainersCourseViewModel viewModel)
        {
            var userTeam = _context.TrainersCourses
                .SingleOrDefault(t => t.CourseId == viewModel.CourseId && t.TrainerId == viewModel.TrainerId);
            if (userTeam == null)
            {
                return HttpNotFound();
            }

            _context.TrainersCourses.Remove(userTeam);
            _context.SaveChanges();

            return RedirectToAction("Index", "Courses");
        }

    }
}