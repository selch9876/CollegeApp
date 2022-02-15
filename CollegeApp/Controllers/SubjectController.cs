using CollegeApp.Data;
using CollegeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CollegeApp.Controllers
{
    public class SubjectController : Controller
    {
        private readonly AppDbContext _db;

        public SubjectController(AppDbContext db)
        {
            _db = db;
        }
       

        public async Task<IActionResult> Index()
        {
            var subjects = _db.Subjects
                .Include(c => c.Course)
                .Include(t => t.Teacher)
                .AsNoTracking();
            return View(await subjects.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _db.Subjects

                .Include(c => c.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        //GET
        public IActionResult Create()
        {
            PopulateCoursesDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
        [Bind("SubjectTitle,CourseId")] Subject obj)
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
            catch (DbUpdateException  /* ex */ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            PopulateCoursesDropDownList(obj.CourseId);
            return View(obj);
        }



        //GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _db.Subjects
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return NotFound();
            }
            PopulateCoursesDropDownList(subject.CourseId);
            return View(subject);
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

            var subjectToUpdate = await _db.Subjects
                .FirstOrDefaultAsync(c => c.Id == id);

            if (await TryUpdateModelAsync<Subject>(subjectToUpdate,
                "",
                c => c.SubjectTitle, c => c.Teacher, c => c.CourseId))
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
            PopulateCoursesDropDownList(subjectToUpdate.CourseId);
            return View(subjectToUpdate);
        }



        //GET
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _db.Subjects
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(subject);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _db.Subjects.FindAsync(id);
            if (subject == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _db.Subjects.Remove(subject);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        

        private void PopulateCoursesDropDownList(object? selectedCourse = null)
        {
            var coursesQuery = from d in _db.Courses
                                   orderby d.Title 
                                   select d;
            ViewBag.CourseId = new SelectList(coursesQuery.AsNoTracking(), "Id", "Title", selectedCourse);
        }
    }
}
