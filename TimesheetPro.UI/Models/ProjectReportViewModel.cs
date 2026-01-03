using System.ComponentModel.DataAnnotations;

namespace TimesheetPro.UI.Models
{
    public class ProjectReportViewModel
    {
        public Guid ProjectId { get; set; }
        [Display(Name = "Project")]
        public string ProjectName { get; set; } = string.Empty;
        [Display(Name = "Client")]
        public string ClientName {  get; set; } = string.Empty;
        [Display(Name = "Budget Hours")]
        public decimal BudgetHours { get; set; }
        [Display(Name = "Approved Hours")]
        public decimal ApprovedHours { get; set; }

        [Display(Name = "Remaining Hours")]
        public decimal RemainingHours => BudgetHours - ApprovedHours;
        [Display(Name = "Budget Status")]
        public bool IsOverBudget => ApprovedHours > BudgetHours;
    }
}
