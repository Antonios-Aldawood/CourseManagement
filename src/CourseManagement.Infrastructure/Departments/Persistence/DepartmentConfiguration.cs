using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagement.Domain.Departments;
using CourseManagement.Domain.Jobs;

namespace CourseManagement.Infrastructure.Departments.Persistence
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(x => x.Id)
                .IsClustered(true);

            builder.Property(x => x.Id)
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(x => x.Name)
                .IsRequired(true)
                .HasColumnName("name");

            builder.HasIndex(x => x.Name)
                .IsUnique();

            builder.Property(x => x.MinMembers)
                .IsRequired(true)
                .HasColumnName("min_members");

            builder.Property(x => x.MaxMembers)
                .IsRequired(true)
                .HasColumnName("max_members");

            builder.Property(x => x.Description)
                .IsRequired(true)
                .HasColumnName("description");

            builder.HasMany<Job>()
                .WithOne()
                .HasForeignKey(j => j.DepartmentId);

            builder.Ignore(x => x.JobIds);
        }
    }
}
