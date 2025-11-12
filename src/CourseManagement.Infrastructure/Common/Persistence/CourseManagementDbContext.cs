using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.Departments;
using CourseManagement.Domain.Jobs;
using CourseManagement.Domain.Roles;
using CourseManagement.Domain.Users;
using CourseManagement.Domain.Courses;
using CourseManagement.Domain.Skills;
using CourseManagement.Domain.CoursesSkills;
using CourseManagement.Domain.JobsSkills;
using CourseManagement.Domain.Enrollments;
using CourseManagement.Application.Users.Common.Token;
//using System.Reflection;
using CourseManagement.Infrastructure.Departments.Persistence;
using CourseManagement.Infrastructure.Jobs.Persistence;
using CourseManagement.Infrastructure.Users.Persistence;
using CourseManagement.Infrastructure.Addresses.Persistence;
using CourseManagement.Infrastructure.Roles.Persistence;
using CourseManagement.Infrastructure.Privileges.Persistence;
using CourseManagement.Infrastructure.RolePrivilege.Persistence;
using CourseManagement.Infrastructure.Materials.Persistence;
using CourseManagement.Infrastructure.Sessions.Persistence;
using CourseManagement.Infrastructure.Eligibilities.Persistence;
using CourseManagement.Infrastructure.Courses.Persistence;
using CourseManagement.Infrastructure.CourseSkill.Persistence;
using CourseManagement.Infrastructure.Skills.Persistence;
using CourseManagement.Infrastructure.JobSkill.Persistence;
using CourseManagement.Infrastructure.Enrollments.Persistence;
using CourseManagement.Infrastructure.Attendances.Persistence;
using CourseManagement.Infrastructure.RefreshTokens.Persistence;

namespace CourseManagement.Infrastructure.Common.Persistence
{
    public class CourseManagementDbContext : DbContext, IUnitOfWork
    {
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Job> Jobs { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Privilege> Privileges { get; set; } = null!;
        public DbSet<RolesPrivileges> RolesPrivileges { get; set; } = null!;
        public DbSet<Material> Materials { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;
        public DbSet<Eligibility> Eligibilities { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<CoursesSkills> CoursesSkills { get; set; } = null!;
        public DbSet<Skill> Skills { get; set; } = null!;
        public DbSet<JobsSkills> JobsSkills { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<Attendance> Attendances { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        public CourseManagementDbContext(DbContextOptions<CourseManagementDbContext> options) : base(options)
        {
        }

        public async Task CommitChangesAsync()
        {
            await SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Application.Logs.Entity.Log>().Metadata.SetIsTableExcludedFromMigrations(true);

            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new JobConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new PrivilegeConfiguration());
            modelBuilder.ApplyConfiguration(new RolesPrivilegesConfiguration());
            modelBuilder.ApplyConfiguration(new MaterialConfiguration());
            modelBuilder.ApplyConfiguration(new SessionConfiguration());
            modelBuilder.ApplyConfiguration(new EligibilityConfiguration());
            modelBuilder.ApplyConfiguration(new CourseConfiguration());
            modelBuilder.ApplyConfiguration(new CoursesSkillsConfiguration());
            modelBuilder.ApplyConfiguration(new SkillConfiguration());
            modelBuilder.ApplyConfiguration(new JobsSkillsConfiguration());
            modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
            modelBuilder.ApplyConfiguration(new AttendanceConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
  
            modelBuilder.Ignore<Application.Logs.Entity.Log>();

            base.OnModelCreating(modelBuilder);
        }
    }
}

