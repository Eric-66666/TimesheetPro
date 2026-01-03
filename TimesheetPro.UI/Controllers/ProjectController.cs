using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimesheetPro.Core.Domain.Entities;
using TimesheetPro.Core.Enums;
using TimesheetPro.Infrastructure.Data;

namespace TimesheetPro.UI.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly TimesheetProDbContext _db;

        //service injection
        public ProjectController(TimesheetProDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var project = await _db.Projects.ToListAsync();
            return View(project);
        }

        [Authorize(Roles = nameof(AppRoles.ProjectManager))]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]//XSRF defence
        [Authorize(Roles = nameof(AppRoles.ProjectManager))]
        public async Task<IActionResult> Create(Project project)
        {
            //form validation
            ValidateDates(project);

            if (!ModelState.IsValid)
            {
                return View(project);
            }

            project.Id = Guid.NewGuid();
            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = nameof(AppRoles.ProjectManager))]
        public async Task<IActionResult> Edit(Guid id)
        {
            var project = await _db.Projects.FirstOrDefaultAsync(x => x.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(AppRoles.ProjectManager))]
        public async Task<IActionResult> Edit(Project project)
        {
            //form validation
            ValidateDates(project);

            if (!ModelState.IsValid)
            {
                return View(project);
            }

            _db.Projects.Update(project);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = nameof(AppRoles.ProjectManager))]
        public async Task<IActionResult> Delete(Guid id)
        {
            var project = await _db.Projects.FirstOrDefaultAsync(x => x.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(AppRoles.ProjectManager))]
        public async Task<IActionResult> Delete(Project model)
        {
            var project = await _db.Projects.FirstOrDefaultAsync(x => x.Id == model.Id);

            if (project == null)
            {
                return NotFound();
            }


            //Check if any timesheets reference this project(cascade)
            //if so, deletion is not allowed
            var hasTimesheets = await _db.TimesheetEntries.AnyAsync(t => t.ProjectId == model.Id);
            if (hasTimesheets)
            {
                TempData["ErrorMessage"] = "This project has timesheets and cannot be deleted";
                return RedirectToAction(nameof(Index));
            }


            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();


            TempData["SuccessMessage"] = "Project was deleted successfully";
            return RedirectToAction(nameof(Index));
        }



        //Form validation: End date cannot be earlier than Start date
        private void ValidateDates(Project project)
        {
            if (project.StartDate.HasValue && project.EndDate.HasValue)
            {
                if (project.StartDate > project.EndDate)
                {
                    ModelState.AddModelError(nameof(project.EndDate), "End date cannot be earlier than Start date");
                }
            }
        }
    }
}
