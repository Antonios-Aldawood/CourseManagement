using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagement.Domain.Courses;

namespace CourseManagement.Infrastructure.Eligibilities.Persistence
{
    public class EligibilityConfiguration : IEntityTypeConfiguration<Eligibility>
    {
        public void Configure(EntityTypeBuilder<Eligibility> builder)
        {
            builder.HasKey(x => x.Id)
                .IsClustered(true);

            builder.Property(x => x.Id)
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(x => x.CourseId)
                .IsRequired(true)
                .HasColumnName("course_id");

            builder.HasOne<Course>()
                .WithMany(c => c.Eligibilities)
                .HasForeignKey(x => x.CourseId);

            builder.Property(x => x.Key)
                .IsRequired(true)
                .HasColumnName("key");

            builder.Property(x => x.Value)
                .IsRequired(true)
                .HasColumnName("value");
        }
    }
}
