using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagement.Domain.Enrollments;
using CourseManagement.Domain.Courses;

namespace CourseManagement.Infrastructure.Attendances.Persistence
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.HasKey(x => x.Id)
                .IsClustered(true);

            builder.Property(x => x.Id)
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(x => x.EnrollmentId)
                .IsRequired(true)
                .HasColumnName("enrollment_id");

            builder.HasOne<Enrollment>()
                .WithMany(e => e.Attendances)
                .HasForeignKey(x => x.EnrollmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.SessionId)
                .IsRequired(true)
                .HasColumnName("session_id");

            builder.HasOne<Session>()
                .WithMany()
                .HasForeignKey(x => x.SessionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.DateAttended)
                .IsRequired(true)
                .HasColumnName("date_attended");
        }
    }
}
