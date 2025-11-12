using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Application.Logs.Entity;
using CourseManagement.Infrastructure.Logs.Persistence;
//using System.Reflection;

namespace CourseManagement.Infrastructure.Logging.Persistence
{
    public class LoggingDbContext : DbContext, ILogUnitOfWork
    {
        public DbSet<Log> Logs { get; set; }

        public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options)
        {
        }

        public async Task CommitChangesAsync()
        {
            await SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.ApplyConfiguration(new LogConfiguration());

            modelBuilder.Entity<Domain.Users.User>().Metadata.SetIsTableExcludedFromMigrations(true);
            
            //modelBuilder.Model.GetEntityTypes()
            //    .Where(t => t.ClrType.Namespace != typeof(Log).Namespace)
            //    .ToList()
            //    .ForEach(t => modelBuilder.Ignore(t.ClrType));

            modelBuilder.Ignore<Domain.Departments.Department>();
            modelBuilder.Ignore<Domain.Jobs.Job>();
            modelBuilder.Ignore<Domain.Users.User>();
            modelBuilder.Ignore<Domain.Users.Address>();
            modelBuilder.Ignore<Domain.Roles.Role>();
            modelBuilder.Ignore<Domain.Roles.RolesPrivileges>();
            modelBuilder.Ignore<Domain.Roles.Privilege>();
            modelBuilder.Ignore<Domain.Courses.Material>();
            modelBuilder.Ignore<Domain.Courses.Session>();
            modelBuilder.Ignore<Domain.Courses.Eligibility>();
            modelBuilder.Ignore<Domain.Courses.Course>();
            modelBuilder.Ignore<Domain.Skills.Skill>();
            modelBuilder.Ignore<Domain.CoursesSkills.CoursesSkills>();
            modelBuilder.Ignore<Domain.JobsSkills.JobsSkills>();
            modelBuilder.Ignore<Domain.Enrollments.Enrollment>();
            modelBuilder.Ignore<Domain.Enrollments.Attendance>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
