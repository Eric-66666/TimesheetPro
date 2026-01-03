using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimesheetPro.Core.Domain.IdentityEntities;
using TimesheetPro.Core.Enums;

namespace TimesheetPro.Core.Domain.Entities
{
    public class TimesheetEntry
    {
        public Guid Id { get; set; }


        //core
        [Required]
        public Guid UserId { get; set; } // AspNetUsers.Id
        [Required]
        public Guid ProjectId { get; set; } // foreign key to Project
        [Required]
        public DateTime WorkDate { get; set; }
        [Range(0.5, 24)]
        public decimal Hours { get; set; }
        public string? Description { get; set; }


        //system
        public TimesheetStatus Status { get; set; } = TimesheetStatus.Draft;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SubmittedAt { get; set; }



        //navigation(.Include operation)
        public Project? Project { get; set; }
        public ApplicationUser? User {  get; set; }

    }
}
