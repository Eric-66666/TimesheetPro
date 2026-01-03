using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TimesheetPro.Core.Enums;
using TimesheetPro.Infrastructure.Data;
using TimesheetPro.UI.Models;

namespace TimesheetPro.UI.Controllers
{
    [Authorize(Roles = $"{nameof(AppRoles.ProjectManager)},{nameof(AppRoles.Finance)}")]
    public class ReportController : Controller
    {
        private readonly TimesheetProDbContext _db;

        public ReportController(TimesheetProDbContext db) 
        {
            _db = db;
        }


        public async Task<IActionResult> Index()
        {
            var report = await GetProjectReportAsync();

            return View(report);
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var report = await GetProjectReportAsync();

            //excel
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Project Report");

            //header
            worksheet.Cells[1, 1].Value = "Client";
            worksheet.Cells[1, 2].Value = "Project";
            worksheet.Cells[1, 3].Value = "Budget Hours";
            worksheet.Cells[1, 4].Value = "Approved Hours";
            worksheet.Cells[1, 5].Value = "Remaining Hours";
            worksheet.Cells[1, 6].Value = "Budget Status";

            //data
            var row = 2;
            foreach (var data in report)
            {
                worksheet.Cells[row, 1].Value = data.ClientName;
                worksheet.Cells[row, 2].Value = data.ProjectName;
                worksheet.Cells[row, 3].Value = data.BudgetHours;
                worksheet.Cells[row, 4].Value = data.ApprovedHours;
                worksheet.Cells[row, 5].Value = data.RemainingHours;
                worksheet.Cells[row, 6].Value = data.IsOverBudget ? "Over budget" : "Within budget";

                row++;
            }

            //layout
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            //return file
            var fileName = $"ProjectReport_{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}.xlsx";
            var bytes = package.GetAsByteArray();

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }




        private async Task<List<ProjectReportViewModel>> GetProjectReportAsync()
        {
            var report = await _db.Projects
                .Select(project => new ProjectReportViewModel
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    ClientName = project.ClientName,
                    BudgetHours = project.BudgetHours,

                    ApprovedHours = _db.TimesheetEntries
                    .Where(t => t.ProjectId == project.Id && t.Status == TimesheetStatus.Approved)
                    .Sum(t => (decimal?)t.Hours) ?? 0
                })
                .OrderBy(projectReport => projectReport.ClientName)
                .ThenBy(projectReport => projectReport.ProjectName)
                .ToListAsync();

            return report;
        }
    }
}
