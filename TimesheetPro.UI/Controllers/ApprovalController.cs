using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimesheetPro.Core.Domain.IdentityEntities;
using TimesheetPro.Core.Enums;
using TimesheetPro.Core.Serivces;
using TimesheetPro.Infrastructure.Data;

namespace TimesheetPro.UI.Controllers
{
    [Authorize(Roles = nameof(AppRoles.ProjectManager))]
    public class ApprovalController : Controller
    {
        private readonly TimesheetProDbContext _db;
        private readonly TimesheetService _timesheetService;

        public ApprovalController(
            TimesheetProDbContext db,
            TimesheetService timesheetService) 
        {
            _db = db;
            _timesheetService = timesheetService;
        }

        //only for "Submitted"
        public async Task<IActionResult> Index()
        {
            var timesheets = await _db.TimesheetEntries
                .Include(t => t.Project)
                .Include(t => t.User)
                .Where(t => t.Status == TimesheetStatus.Submitted)
                .OrderByDescending(t => t.WorkDate)
                .ToListAsync();

            return View(timesheets);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id)
        {
            var entry = await _db.TimesheetEntries.FirstOrDefaultAsync(t => t.Id == id);

            if (entry == null || entry.Status != TimesheetStatus.Submitted)
            {
                return NotFound();
            }


            try
            {
                _timesheetService.Approve(entry);
                
                await _db.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }


            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id)
        {
            var entry =  await _db.TimesheetEntries.FirstOrDefaultAsync(t => t.Id == id);

            if (entry == null || entry.Status != TimesheetStatus.Submitted)
            {
                return NotFound();
            }

            try
            {
                _timesheetService.Reject(entry);

                await _db.SaveChangesAsync();
            }
            catch(InvalidOperationException)
            {
                return BadRequest();
            }


            return RedirectToAction(nameof(Index));
        }

    }
}
