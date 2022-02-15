using CollegeApp.Data;
using CollegeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace CollegeApp.Controllers
{
    public class TeacherController : Controller
    {
        private readonly AppDbContext _db;

        public TeacherController(AppDbContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> Index()
        {
            var teachers = _db.Teachers
                .Include(c => c.Subject)
                .AsNoTracking();
            return View(await teachers.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _db.Teachers
                .Include(s => s.Subject)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.TeacherId == id);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
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
         Teacher obj)
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
                    "A teacher has already been assigned to this subject. ");
            }
            PopulateSubjectsDropDownList(obj.SubjectID);
            return View(obj);
        }



        //GET 
           public async Task<IActionResult> Edit(int? id)
           {
               if (id == null)
               {
                   return NotFound();
               }

                var teacher = await _db.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.TeacherId == id);
            if (teacher == null)
               {
                   return NotFound();
               }
               PopulateSubjectsDropDownList(teacher.SubjectID);
               return View(teacher);
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

               var teacherToUpdate = await _db.Teachers
                   .FirstOrDefaultAsync(c => c.TeacherId == id);
            

            if (await TryUpdateModelAsync<Teacher>(teacherToUpdate,
                   "",
                   c => c.TeacherName, c => c.SubjectID, c => c.Salary, c => c.Birthday))
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
               PopulateSubjectsDropDownList(teacherToUpdate.SubjectID);
               return View(teacherToUpdate);
           }



        //GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var teacherFromDb = _db.Teachers.Find(id);
           
            if (teacherFromDb == null)
            {
                return NotFound();
            }
            return View(teacherFromDb);
        }


        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _db.Teachers.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Teachers.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Teacher deleted successfully.";
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
