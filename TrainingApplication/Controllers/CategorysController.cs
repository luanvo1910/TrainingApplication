using TrainingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrainingApplication.Utils;

namespace TrainingApplication.Controllers
{
    [Authorize(Roles = Role.Staff)]
    public class CategorysController : Controller
    {
        private ApplicationDbContext _context;
        public CategorysController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Category
        [HttpGet]
        public ActionResult Index(string SearchCategorys)
        {

            var categorys = _context.Categories.ToList();
            if (!string.IsNullOrEmpty(SearchCategorys))
            {
                categorys = categorys
                    .Where(t => t.Name.ToLower().Contains(SearchCategorys.ToLower())).
                    ToList();
            }
            return View(categorys);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Category cate)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var newCategory = new Category()
            {
                Name = cate.Name,
                Description = cate.Description
            };
            _context.Categories.Add(newCategory);
            _context.SaveChanges();
            return RedirectToAction("Index", "Categorys");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var cateInDb = _context.Categories.SingleOrDefault(t => t.Id == id);
            if (cateInDb == null)
            {
                return HttpNotFound();
            }
            _context.Categories.Remove(cateInDb);
            _context.SaveChanges();
            return RedirectToAction("Index", "Categorys");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {

            var cateInDb = _context.Categories.SingleOrDefault(t => t.Id == id);
            if (cateInDb == null)
            {
                return HttpNotFound();
            }
            return View(cateInDb);
        }
        [HttpPost]
        public ActionResult Edit(Category cate)
        {
            if (!ModelState.IsValid)
            {
                return View(cate);
            }
            var cateInDb = _context.Categories.SingleOrDefault(t => t.Id == cate.Id);
            if (cateInDb == null)
            {
                return HttpNotFound();
            }
            cateInDb.Name = cate.Name;
            cateInDb.Description = cate.Description;
            _context.SaveChanges();
            return RedirectToAction("Index", "Categorys");
        }
    }
}
