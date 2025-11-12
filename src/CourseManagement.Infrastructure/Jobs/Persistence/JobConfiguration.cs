using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagement.Domain.Jobs;
using CourseManagement.Domain.Departments;
using CourseManagement.Domain.Users;
using CourseManagement.Domain.JobsSkills;

namespace CourseManagement.Infrastructure.Jobs.Persistence
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.HasKey(x => x.Id)
                .IsClustered(true);

            builder.Property(x => x.Id)
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(x => x.Title)
                .IsRequired(true)
                .HasColumnName("title");

            builder.HasIndex(x => x.Title)
                .IsUnique();

            builder.Property(x => x.MinSalary)
                .IsRequired(true)
                .HasColumnName("min_salary");

            builder.Property(x => x.MaxSalary)
                .IsRequired(true)
                .HasColumnName("max_salary");

            builder.Property(x => x.Description)
                .IsRequired(true)
                .HasColumnName("description");

            builder.Property(x => x.DepartmentId)
                .IsRequired(true)
                .HasColumnName("department_Id");

            builder.HasOne<Department>()
                .WithMany()
                .HasForeignKey(x => x.DepartmentId);

            builder.HasMany<User>()
                .WithOne()
                .HasForeignKey(u => u.JobId);

            builder.HasMany<JobsSkills>()
                .WithOne()
                .HasForeignKey(js => js.JobId);

            builder.Ignore(x => x.JobSkillsIds);
        }
    }
}
