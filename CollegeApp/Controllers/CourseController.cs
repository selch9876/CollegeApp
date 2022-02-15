using CollegeApp.Data;
using CollegeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeApp.Controllers
{
    public class CourseController : Controller
    {
        private readonly AppDbContext _db;

        public CourseController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            //IEnumerable<Course> objCourseList = _db.Courses.ToList();
            //return View(objCourseList);
            var courses = _db.Courses
            .Include(c => c.subjects)
            .AsNoTracking();
            return View(await courses.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _db.Courses
                .Include(s => s.subjects)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Course obj)
        {

            if (ModelState.IsValid)
            {
                _db.Courses.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Course created successfully.";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var courseFromDb = _db.Courses.Find(id);
           
            if (courseFromDb == null)
            {
                return NotFound();
            }
            return View(courseFromDb);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Course obj)
        {

            if (ModelState.IsValid)
            {
                _db.Courses.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Course updated successfully.";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var courseFromDb = _db.Courses.Find(id);
            //var categoryFromDbFirst = _db.categories.FirstOrDefault(c => c.Id == id);
            //var categoryFromDbSingle = _db.categories.SingleOrDefault(c => c.Id == id);
            if (courseFromDb == null)
            {
                return NotFound();
            }
            return View(courseFromDb);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[ActionName("Delete")] // We gave a custom name for the action method
        public IActionResult DeletePOST(int? id)
        {
            var obj = _db.Courses.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Courses.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Course deleted successfully.";
            return RedirectToAction("Index");


        }
    }
}
