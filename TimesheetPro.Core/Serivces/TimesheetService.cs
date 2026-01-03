using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimesheetPro.Core.Domain.Entities;
using TimesheetPro.Core.Enums;

namespace TimesheetPro.Core.Serivces
{
    public class TimesheetService
    {
        //Status: Draft -> Submitted
        public void Submit(TimesheetEntry timesheetEntry, DateTime utcNow)
        {
            if (timesheetEntry == null)
                throw new ArgumentNullException(nameof(timesheetEntry));

            if (timesheetEntry.Status != TimesheetStatus.Draft)
                throw new InvalidOperationException("Only draft timesheets can be submitted");

            timesheetEntry.Status = TimesheetStatus.Submitted;
            timesheetEntry.SubmittedAt = utcNow;
        }

        //Status: Submitted -> Approved
        public void Approve(TimesheetEntry timesheetEntry)
        {
            if (timesheetEntry == null)
                throw new ArgumentNullException(nameof(timesheetEntry));

            if (timesheetEntry.Status != TimesheetStatus.Submitted)
                throw new InvalidOperationException("Only submitted timesheets can be approved");

            timesheetEntry.Status = TimesheetStatus.Approved;
        }

        //Status: Submitted -> Rejected
        public void Reject(TimesheetEntry timesheetEntry)
        {
            if (timesheetEntry == null)
                throw new ArgumentNullException(nameof(timesheetEntry));
            if (timesheetEntry.Status != TimesheetStatus.Submitted)
                throw new InvalidOperationException("Only submitted timesheets can be rejected");

            timesheetEntry.Status = TimesheetStatus.Rejected;
        }
    }
}
