using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimesheetPro.Core.Domain.Entities;
using TimesheetPro.Core.Domain.IdentityEntities;
using TimesheetPro.Core.Enums;
using TimesheetPro.Core.Serivces;
using TimesheetPro.Infrastructure.Data;

namespace TimesheetPro.UI.Controllers
{
    //only for Consultant
    [Authorize(Roles = nameof(AppRoles.Consultant))]
    public class TimesheetController : Controller
    {

        private readonly TimesheetProDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TimesheetService _timesheetService;

        public TimesheetController(
            TimesheetProDbContext db,
            UserManager<ApplicationUser> userManager,
            TimesheetService timesheetService)
        {
            _db = db;
            _userManager = userManager;
            _timesheetService = timesheetService;
        }

        //only for current user
        public async Task<IActionResult> MyTimesheet()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var userGuid = Guid.Parse(userId);

            var timesheet = await _db.TimesheetEntries
                .Include(t => t.Project)
                .Where(t => t.UserId == userGuid)
                .OrderByDescending(t => t.WorkDate)
                .ToListAsync();

            return View(timesheet);
        }


        public async Task<IActionResult> Create()
        {
            ViewBag.Projects = await _db.Projects
                .OrderBy(p => p.Name)
                .ToListAsync();

            var timesheetEntry = new TimesheetEntry
            {
                WorkDate = DateTime.Today
            };

            return View(timesheetEntry);
        }

        //only for current user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TimesheetEntry timesheetEntry)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Projects = await _db.Projects
                .OrderBy(p => p.Name)
                .ToListAsync();

                return View(timesheetEntry);
            }


            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var userGuid = Guid.Parse(userId);


            //system
            timesheetEntry.Id = Guid.NewGuid();
            timesheetEntry.UserId = userGuid;
            timesheetEntry.Status = TimesheetStatus.Draft;
            timesheetEntry.CreatedAt = DateTime.UtcNow;

            _db.TimesheetEntries.Add(timesheetEntry);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(MyTimesheet));
        }


        //Only allow editing of your own draft records
        public async Task<IActionResult> Edit(Guid id)
        {
            var usedId = _userManager.GetUserId(User);
            if (usedId == null)
            {
                return Unauthorized();
            }
            var userGuid = Guid.Parse(usedId);


            var timesheetEntry = await _db.TimesheetEntries
                .FirstOrDefaultAsync(t => t.Id == id 
                && t.UserId == userGuid 
                && t.Status == TimesheetStatus.Draft);


            if (timesheetEntry == null)
            {
                return NotFound();
            }


            ViewBag.Projects = await _db.Projects
                .OrderBy(p => p.Name)
                .ToListAsync();

            return View(timesheetEntry);
        }


        //Only allow editing of your own draft records
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TimesheetEntry timesheetEntry)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Projects = await _db.Projects
                .OrderBy(p => p.Name)
                .ToListAsync();

                return View(timesheetEntry);
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var userGuid = Guid.Parse(userId);

            var userEntry = await _db.TimesheetEntries
                .FirstOrDefaultAsync(t => t.Id == timesheetEntry.Id
                && t.UserId == userGuid
                && t.Status == TimesheetStatus.Draft);

            if (userEntry == null)
            {
                return NotFound();
            }



            userEntry.ProjectId = timesheetEntry.ProjectId;
            userEntry.WorkDate = timesheetEntry.WorkDate;
            userEntry.Hours = timesheetEntry.Hours;
            userEntry.Description = timesheetEntry.Description;


            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(MyTimesheet));

        }

        //Only your own draft records are allowed to be submitted
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(Guid id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var userGuid = Guid.Parse(userId);

            var timesheetEntry = await _db.TimesheetEntries
                .FirstOrDefaultAsync(t => t.Id == id
                && t.UserId == userGuid
                && t.Status == TimesheetStatus.Draft);

            if (timesheetEntry == null)
            {
                return NotFound();
            }


            try
            {
                _timesheetService.Submit(timesheetEntry, DateTime.UtcNow);

                await _db.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }


            return RedirectToAction(nameof(MyTimesheet));
        }
    }
}
