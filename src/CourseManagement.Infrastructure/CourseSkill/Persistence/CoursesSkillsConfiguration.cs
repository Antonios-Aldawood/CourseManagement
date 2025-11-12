using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagement.Domain.CoursesSkills;
using CourseManagement.Domain.Courses;
using CourseManagement.Domain.Skills;

namespace CourseManagement.Infrastructure.CourseSkill.Persistence
{
    public class CoursesSkillsConfiguration : IEntityTypeConfiguration<CoursesSkills>
    {
        public void Configure(EntityTypeBuilder<CoursesSkills> builder)
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
                .WithMany()
                .HasForeignKey(x => x.CourseId);

            builder.Property(x => x.SkillId)
                .IsRequired(true)
                .HasColumnName("skill_id");

            builder.HasOne<Skill>()
                .WithMany()
                .HasForeignKey(x => x.SkillId);

            builder.Property(x => x.Weight)
                .IsRequired(true)
                .HasColumnName("weight");
        }
    }
}
