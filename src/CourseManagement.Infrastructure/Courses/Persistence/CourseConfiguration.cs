using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagement.Domain.Courses;
using CourseManagement.Domain.CoursesSkills;
using CourseManagement.Domain.Enrollments;

namespace CourseManagement.Infrastructure.Courses.Persistence
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(x => x.Id)
                .IsClustered(true);

            builder.Property(x => x.Id)
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(x => x.Subject)
                .IsRequired(true)
                .HasColumnName("subject");

            builder.HasIndex(x => x.Subject)
                .IsUnique();

            builder.Property(x => x.Description)
                .IsRequired(true)
                .HasColumnName("description");

            builder.HasMany(x => x.Eligibilities)
                .WithOne()
                .HasForeignKey(e => e.CourseId);

            builder.Property(x => x.IsForAll)
                .IsRequired(true)
                .HasColumnName("is_for_all");

            builder.HasMany<CoursesSkills>()
                .WithOne()
                .HasForeignKey(cs => cs.CourseId);

            builder.HasMany(x => x.Sessions)
                .WithOne()
                .HasForeignKey(s => s.CourseId);

            builder.Ignore(x => x.CoursesSkillsIds);

            builder.HasMany<Enrollment>()
                .WithOne()
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
