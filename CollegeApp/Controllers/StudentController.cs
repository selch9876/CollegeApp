using CollegeApp.Data;
using CollegeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CollegeApp.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _db;

        public StudentController(AppDbContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> Index()
        {
            var students = _db.Students
                .Include(c => c.Subjects)
                .AsNoTracking();
            return View(await students.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _db.Students
                .Include(s => s.Subjects)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        //GET
        public IActionResult Create()
        {
            PopulateSubjectsDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
         Student obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Add(obj);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {

                ModelState.AddModelError("", "Unable to save changes. " +
                    "A student has already been assigned. ");
            }
            PopulateSubjectsDropDownList(obj.Subjects);
            return View(obj);
        }



        //GET 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }
            PopulateSubjectsDropDownList(student.Subjects);
            return View(student);
        }

        //POST
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentToUpdate = await _db.Students
                .FirstOrDefaultAsync(c => c.Id == id);


            if (await TryUpdateModelAsync<Student>(studentToUpdate,
                   "",
                   c => c.StudentName, c => c.GradeId, c => c.Birthday))
            {

                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateSubjectsDropDownList(studentToUpdate.Subjects);
            return View(studentToUpdate);
        }



        //GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var studentFromDb = _db.Students.Find(id);

            if (studentFromDb == null)
            {
                return NotFound();
            }
            return View(studentFromDb);
        }


        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _db.Students.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Students.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Student deleted successfully.";
            return RedirectToAction("Index");

        }

        private void PopulateSubjectsDropDownList(object? selectedSubject = null)
        {
            var subjectQuery = from d in _db.Subjects
                               orderby d.SubjectTitle
                               select d;
            ViewBag.SubjectID = new SelectList(subjectQuery.AsNoTracking(), "Id", "SubjectTitle", selectedSubject);
        }
    }
}

