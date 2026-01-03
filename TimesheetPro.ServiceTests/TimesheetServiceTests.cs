using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimesheetPro.Core.Domain.Entities;
using TimesheetPro.Core.Enums;
using TimesheetPro.Core.Serivces;

namespace TimesheetPro.ServiceTests
{
    public class TimesheetServiceTests
    {
        private readonly TimesheetService _service;

        public TimesheetServiceTests()
        {
            _service = new TimesheetService();
        }

        private TimesheetEntry CreateDraftEntry()
        {
            return new TimesheetEntry 
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                WorkDate = new DateTime(2025,1,1),
                Hours = 8,
                Status = TimesheetStatus.Draft
            };
        }


        #region Submit
        [Fact]
        public void Submit_ChangesStatusToSubmitted_ToBeSuccessful()
        {
            //Arrange
            var timesheetEntry = CreateDraftEntry();
            var now = new DateTime(2025, 1, 7, 12, 0, 0, DateTimeKind.Utc);

            //Act
            _service.Submit(timesheetEntry, now);

            //Assert
            Assert.Equal(TimesheetStatus.Submitted, timesheetEntry.Status);
            Assert.Equal(now, timesheetEntry.SubmittedAt);
        }

        [Theory]
        [InlineData(TimesheetStatus.Submitted)]
        [InlineData(TimesheetStatus.Approved)]
        [InlineData(TimesheetStatus.Rejected)]
        public void Submit_NonDraft_ThrowInvalidOperationException(TimesheetStatus status)
        {
            var timesheetEntry = CreateDraftEntry();
            var now = new DateTime(2025, 1, 7, 12, 0, 0, DateTimeKind.Utc);
            timesheetEntry.Status = status;


            Assert.Throws<InvalidOperationException>(
                () => _service.Submit(timesheetEntry,now));
        }

        #endregion

        #region Approve

        [Fact]
        public void Approve_ChangesStatusToApproved_ToBeSuccessful()
        {
            var timesheetEntry = CreateDraftEntry();
            timesheetEntry.Status = TimesheetStatus.Submitted;

            _service.Approve(timesheetEntry);

            Assert.Equal(TimesheetStatus.Approved, timesheetEntry.Status);
        }

        [Theory]
        [InlineData(TimesheetStatus.Draft)]
        [InlineData(TimesheetStatus.Approved)]
        [InlineData(TimesheetStatus.Rejected)]
        public void Approve_NonSubmitted_ThrowInvalidOperationException(TimesheetStatus status)
        {
            var timesheetEntry = CreateDraftEntry();
            timesheetEntry.Status = status;

            Assert.Throws<InvalidOperationException>(
                () => _service.Approve(timesheetEntry));
        }
        #endregion

        #region Reject

        [Fact]
        public void Reject_ChangesStatusToRejected_ToBeSuccessful()
        {
            var timesheetEntry = CreateDraftEntry();
            timesheetEntry.Status = TimesheetStatus.Submitted;

            _service.Reject(timesheetEntry);

            Assert.Equal(TimesheetStatus.Rejected, timesheetEntry.Status);
        }

        [Theory]
        [InlineData(TimesheetStatus.Draft)]
        [InlineData(TimesheetStatus.Approved)]
        [InlineData(TimesheetStatus.Rejected)]
        public void Reject_NonSubmitted_ThrowInvalidOperationException(TimesheetStatus status)
        {
            var timesheetEntry = CreateDraftEntry();
            timesheetEntry.Status = status;

            Assert.Throws<InvalidOperationException>(
                () => _service.Reject(timesheetEntry));
        }
        #endregion
    }
}
