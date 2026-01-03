using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TimesheetPro.Core.Domain.Entities;
using TimesheetPro.Core.Domain.IdentityEntities;

namespace TimesheetPro.Infrastructure.Data
{
    public class TimesheetProDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
    {
        public TimesheetProDbContext(DbContextOptions options) : base(options)
        {
        }

        //Connect with database
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<TimesheetEntry> TimesheetEntries => Set<TimesheetEntry>();



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Configure table name
            modelBuilder.Entity<Project>().ToTable("Projects");
            modelBuilder.Entity<TimesheetEntry>().ToTable("TimesheetEntries");
        }
    }
}
