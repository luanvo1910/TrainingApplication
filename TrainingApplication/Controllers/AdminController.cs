using TrainingApplication.Models;
using TrainingApplication.Utils;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TrainingApplication.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using static TrainingApplication.Controllers.ManageController;

namespace TrainingApplication.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class AdminController : Controller
    {
        private ApplicationDbContext _context;
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        public AdminController()
        {
            _context = new ApplicationDbContext();
        }
        public AdminController(ApplicationUserManager userManager)
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
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateStaff()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateStaff(CreateStaffViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = viewModel.Email, Email = viewModel.Email };
                var result = await UserManager.CreateAsync(user, viewModel.Password);
                var StaffId = user.Id;
                var newStaff = new Staff()
                {
                    Email = viewModel.Email,
                    StaffId = StaffId,
                    Name = viewModel.Name,
                    Age = viewModel.Age,
                    Address = viewModel.Address
                };
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, Role.Staff);
                    _context.Staffs.Add(newStaff);
                    _context.SaveChanges();

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("GetStaffs", "Admin");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
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
        public ActionResult CreateTrainer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateTrainer(CreateTrainerViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = viewModel.Email, Email = viewModel.Email };
                var result = await UserManager.CreateAsync(user, viewModel.Password);
                var TrainerId = user.Id;
                var newTrainer = new Trainer()
                {
                    TrainerId = TrainerId,
                    Email = viewModel.Email,
                    Name = viewModel.Name,
                    Age = viewModel.Age,
                    Address = viewModel.Address,
                    Specialty = viewModel.Specialty
                };
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, Role.Trainer);
                    _context.Trainers.Add(newTrainer);
                    _context.SaveChanges();

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("GetTrainers", "Admin");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult GetStaffs()
        {
            var staffs = _context.Staffs.ToList();
            return View(staffs);
        }

        [HttpGet]
        public ActionResult DeleteStaff(string id)
        {
            var staffInDb = _context.Users
                .SingleOrDefault(t => t.Id == id);
            var staffInfoInDb = _context.Staffs
                .SingleOrDefault(t => t.StaffId == id);
            if (staffInDb == null || staffInfoInDb == null)
            {
                return HttpNotFound();
            }
            _context.Users.Remove(staffInDb);
            _context.Staffs.Remove(staffInfoInDb);
            _context.SaveChanges();
            return RedirectToAction("GetStaffs", "Admin");
        }

        [HttpGet]
        public ActionResult EditStaff(int id)
        {
            var staffInDb = _context.Staffs
                .SingleOrDefault(t => t.Id == id);
            if (staffInDb == null)
            {
                return HttpNotFound();
            }
            return View(staffInDb);
        }

        [HttpPost]
        public ActionResult EditStaff(Staff staff)
        {
            var staffInDb = _context.Staffs.SingleOrDefault(t => t.Id == staff.Id);
            if (staffInDb == null)
            {
                return HttpNotFound();
            }
            staffInDb.Name = staff.Name;
            staffInDb.Age = staff.Age;
            staffInDb.Address = staff.Address;

            _context.SaveChanges();
            return RedirectToAction("GetStaffs", "Admin");
        }

        public ActionResult StaffChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StaffChangePassword(PasswordViewModel model, string id)
        {
            var userInDb = _context.Users.SingleOrDefault(i => i.Id == id);
            if (userInDb == null)
            {
                return HttpNotFound();
            }
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            userId = userInDb.Id;

            if (userId != null)
            {
                UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
                userManager.RemovePassword(userId);
                string newPassword = model.NewPassword;
                userManager.AddPassword(userId, newPassword);
            }
            _context.SaveChanges();
            return RedirectToAction("GetStaffs", "Admin", new { Message = ManageMessageId.ChangePasswordSuccess });
        }

        [HttpGet]
        public ActionResult GetTrainers()
        {
            var trainers = _context.Trainers.ToList();
            return View(trainers);
        }

        [HttpGet]
        public ActionResult DeleteTrainer(string id)
        {
            var trainerInDb = _context.Users
                .SingleOrDefault(t => t.Id == id);
            var trainerInfoInDb = _context.Trainers
                .SingleOrDefault(t => t.TrainerId == id);
            if (trainerInfoInDb == null || trainerInfoInDb == null)
            {
                return HttpNotFound();
            }
            _context.Users.Remove(trainerInDb);
            _context.Trainers.Remove(trainerInfoInDb);
            _context.SaveChanges();
            return RedirectToAction("GetTrainers", "Admin");
        }

        [HttpGet]
        public ActionResult EditTrainer(int id)
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
        public ActionResult EditTrainer(Trainer trainer)
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
            return RedirectToAction("GetTrainers", "Admin");
        }

        public ActionResult TrainerChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TrainerChangePassword(PasswordViewModel model, string id)
        {
            var userInDb = _context.Users.SingleOrDefault(i => i.Id == id);
            if (userInDb == null)
            {
                return HttpNotFound();
            }
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            userId = userInDb.Id;

            if (userId != null)
            {
                UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
                userManager.RemovePassword(userId);
                string newPassword = model.NewPassword;
                userManager.AddPassword(userId, newPassword);
            }
            _context.SaveChanges();
            return RedirectToAction("GetTrainers", "Admin", new {Message = ManageMessageId.ChangePasswordSuccess });
        }
    }
}