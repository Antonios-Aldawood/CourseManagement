using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagement.Domain.Enrollments;
using CourseManagement.Domain.Users;
using CourseManagement.Domain.Courses;

namespace CourseManagement.Infrastructure.Enrollments.Persistence
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasKey(x => x.Id)
                .IsClustered(true);

            builder.Property(x => x.Id)
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(x => x.UserId)
                .IsRequired(true)
                .HasColumnName("user_id");

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CourseId)
                .IsRequired(true)
                .HasColumnName("course_id");

            builder.HasOne<Course>()
                .WithMany()
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.IsOptional)
                .IsRequired(true)
                .HasColumnName("is_optional");

            builder.Property(x => x.IsCompleted)
                .IsRequired(true)
                .HasColumnName("is_completed");

            builder.HasMany(x => x.Attendances)
                .WithOne()
                .HasForeignKey(a => a.EnrollmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

/*
            //Can be substituted with database queries, but I thought it's faster to keep track of it here.
            builder.Property(x => x.OptionalCompletion)
                .IsRequired(true)
                .HasColumnName("optional_completion");
*/
