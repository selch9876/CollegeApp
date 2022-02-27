using CollegeApp.Data;
using CollegeApp.Models;
using CollegeApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CollegeApp.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _db;

        public StudentController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int? id, int? SubjectId, int? gradeID)
        {
            var viewModel = new StudentIndexData();
            viewModel.Students = _db.Students
                .Include(i => i.Subjects)
                .Include(g => g.Grades)
                .OrderBy(i => i.StudentName);

            if (id != null)
            {
                ViewBag.StudentID = id.Value;
                viewModel.Subjects = viewModel.Students.Single(i => i.Id == id.Value).Subjects;
            }

            if (SubjectId != null)
            {
                ViewBag.SubjectID = SubjectId.Value;
                viewModel.Grades = viewModel.Subjects.Single(x => x.Id == SubjectId.Value).Grades;
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _db.Students
                .Include(s => s.Subjects)
                .ThenInclude(g => g.Grades)
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
            var student = new Student();
            student.Subjects = new List<Subject>();
            PopulateEnrolledSubjectData(student);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
         Student obj, string[] selectedSubjects)
        {
            try
            {
                if (selectedSubjects != null)
                {
                    obj.Subjects = new List<Subject>();
                    foreach (var subject in selectedSubjects)
                    {
                        var subjectToAdd = _db.Subjects.Find(int.Parse(subject));
                        obj.Subjects.Add(subjectToAdd);
                    }
                }


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
                .Include(i => i.Subjects)
                .Include(i => i.Grades)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
                 PopulateEnrolledSubjectData(student);

           

            if (student == null)
            {
                return NotFound();
            }
            //PopulateSubjectsDropDownList(student.Subjects);
            return View(student);
        }

        private void PopulateEnrolledSubjectData(Student student)
        {
            var allSubjects = _db.Subjects;
            var studentSubjects = new HashSet<int>(student.Subjects.Select(c => c.Id));
            //var studentGrades = _db.Grades;     
            var viewModel = new List<EnrolledSubjectData>();
            foreach (var subject in allSubjects)
            {
                var studentGrade = _db.Grades
                    .Where(g => g.StudentId == student.Id && g.SubjectId == subject.Id)
                    .Select(g => g.Value).SingleOrDefault()
                    ;
                var studentGradeId = _db.Grades
                    .Where(g => g.StudentId == student.Id && g.SubjectId == subject.Id)
                    .Select(g => g.GradeId).SingleOrDefault()
                    ;

                viewModel.Add(new EnrolledSubjectData
                {
                    SubjectID = subject.Id,
                    SubjectTitle = subject.SubjectTitle, 
                    Enrolled = studentSubjects.Contains(subject.Id),
                    Grade = studentGrade,
                    GradeId = student.Id
                });
            }


            

            ViewBag.Subjects = viewModel;
        }

        //POST
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, string[] selectedSubjects, int? gradeId, int? selectedGrade)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grade = _db.Grades.Find(gradeId);

            var studentToUpdate = await _db.Students
                .Include(i => i.Subjects)
                .Include(g => g.Grades)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (await TryUpdateModelAsync<Student>(studentToUpdate,
                   "",
                   c => c.StudentName, c => c.Birthday, c => c.Subjects, c => c.Grades))
            {

                try
                {
                    UpdateStudentSubjects(selectedSubjects, studentToUpdate);
                    
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

        private void UpdateStudentSubjects(string[] selectedSubjects, Student studentToUpdate)
        {
            if (selectedSubjects == null)
            {
                studentToUpdate.Subjects = new List<Subject>();
                return;
            }

            
            var selectedSubjectsHS = new HashSet<string>(selectedSubjects);
            var studentSubjects = new HashSet<int>(studentToUpdate.Subjects.Select(c => c.Id));
            

            foreach (var subject in _db.Subjects)
            {
                
                if (selectedSubjectsHS.Contains(subject.Id.ToString()))
                {
                    if (!studentSubjects.Contains(subject.Id))
                    {
                        studentToUpdate.Subjects.Add(subject);
                        
                            _db.SaveChanges();

                    }
                }
                else
                {
                    if (studentSubjects.Contains(subject.Id))
                    {
                        studentToUpdate.Subjects.Remove(subject);
                    }
                }

            }

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

