using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagement.Domain.Courses;
using CourseManagement.Domain.Users;
using CourseManagement.Domain.Enrollments;

namespace CourseManagement.Infrastructure.Sessions.Persistence
{
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
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

            builder.Property(x => x.CourseId)
                .IsRequired(true)
                .HasColumnName("course_id");

            builder.HasOne<Course>()
                .WithMany(c => c.Sessions)
                .HasForeignKey(x => x.CourseId);

            builder.Property(x => x.StartDate)
                .IsRequired(true)
                .HasColumnName("start_date");

            builder.Property(x => x.EndDate)
                .IsRequired(true)
                .HasColumnName("end_date");

            builder.Property(x => x.TrainerId)
                .IsRequired(true)
                .HasColumnName("trainer_id");

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.TrainerId);

            builder.Property(x => x.IsOffline)
                .IsRequired(true)
                .HasColumnName("is_offline");

            builder.Property(x => x.Seats)
                .IsRequired(false)
                .HasColumnName("seats");

            builder.Property(x => x.Link)
                .IsRequired(false)
                .HasColumnName("link");

            builder.Property(x => x.App)
                .IsRequired(false)
                .HasColumnName("app");

            builder.HasMany(x => x.Materials)
                .WithOne()
                .HasForeignKey(m => m.SessionId);

            builder.HasMany<Attendance>()
                .WithOne()
                .HasForeignKey(a => a.SessionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
