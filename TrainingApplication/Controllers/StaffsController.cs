using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TrainingApplication.Models;
using TrainingApplication.Utils;
using TrainingApplication.ViewModels;

namespace TrainingApplication.Controllers
{
    [Authorize(Roles = Role.Staff)]
    public class StaffsController : Controller
    {
        private ApplicationDbContext _context;
        private ApplicationUserManager _userManager;
        public StaffsController()
        {
            _context = new ApplicationDbContext();
        }
        public StaffsController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
            _context = new ApplicationDbContext();
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        // GET: Staffs
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateTrainee()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateTrainee(CreateTraineeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = viewModel.Email, Email = viewModel.Email };
                var result = await UserManager.CreateAsync(user, viewModel.Password);
                var TraineeId = user.Id;
                var newTrainee = new Trainee()
                {
                    TraineeId = TraineeId,
                    Email = viewModel.Email,
                    Name = viewModel.Name,
                    Age = viewModel.Age,
                    DateOfBirth = viewModel.DateOfBirth,
                    Address = viewModel.Address,
                    Education = viewModel.Education
                };
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, Role.Trainee);
                    _context.Trainees.Add(newTrainee);
                    _context.SaveChanges();
                    return RedirectToAction("GetTrainee", "Staffs");
                }
                AddErrors(result);
            }
            return View(viewModel);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [HttpGet]
        public ActionResult GetTrainee(string searchName)
        {
            var trainees = _context.Trainees.ToList();
            if (!string.IsNullOrEmpty(searchName))
            {
                trainees = trainees
                    .Where(t => t.Name.ToLower().Contains(searchName.ToLower())
                    || t.Age.ToString().Contains(searchName.ToLower())
                    ).ToList();
            }
            return View(trainees);
        }

        [HttpGet]
        public ActionResult DeleteTrainee(string id)
        {
            var traineeInDb = _context.Users
                .SingleOrDefault(t => t.Id == id);
            var traineeInfoInDb = _context.Trainees
                .SingleOrDefault(t => t.TraineeId == id);
            if (traineeInDb == null || traineeInfoInDb == null)
            {
                ModelState.AddModelError("", "Trainee is not in thí course");
                return RedirectToAction("GetTrainee", "Staffs");
            }
            _context.Users.Remove(traineeInDb);
            _context.Trainees.Remove(traineeInfoInDb);
            _context.SaveChanges();
            return RedirectToAction("GetTrainee", "Staffs");
        }

        [HttpGet]
        public ActionResult EditTrainee(int id)
        {
            var traineeInDb = _context.Trainees
                .SingleOrDefault(t => t.Id == id);
            if (traineeInDb == null)
            {
                return HttpNotFound();
            }
            return View(traineeInDb);
        }

        [HttpPost]
        public ActionResult EditTrainee(Trainee trainee)
        {
            var traineeInDb = _context.Trainees.SingleOrDefault(t => t.Id == trainee.Id);
            if (traineeInDb == null)
            {
                ModelState.AddModelError("", "Trainee is not Exist");
                return RedirectToAction("GetTrainee", "Staffs");
            }
            traineeInDb.Name = trainee.Name;
            traineeInDb.Age = trainee.Age;
            traineeInDb.DateOfBirth = trainee.DateOfBirth;
            traineeInDb.Address = trainee.Address;
            traineeInDb.Education = trainee.Education;

            _context.SaveChanges();
            return RedirectToAction("GetTrainee", "Staffs");
        }
    }
}