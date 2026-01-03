using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimesheetPro.Core.Domain.Entities
{
    public class Project
    {
        [Key]
        public Guid Id { get; set; }


        //projrct name
        [Required]
        [MaxLength(200)]
        [Display(Name= "Project Name")]
        public string Name { get; set; } = string.Empty;
        //client name
        [Required]
        [MaxLength(200)]
        [Display(Name = "Client")]
        public string ClientName { get; set; } = string.Empty;
        //expected project hours
        [Display(Name = "Budget Hours")]
        public decimal BudgetHours { get; set; }


        //project details
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
    }
}
