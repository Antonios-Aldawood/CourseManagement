using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagement.Domain.Skills;
using CourseManagement.Domain.CoursesSkills;
using CourseManagement.Domain.JobsSkills;

namespace CourseManagement.Infrastructure.Skills.Persistence
{
    public class SkillConfiguration : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.HasKey(x => x.Id)
                .IsClustered(true);

            builder.Property(x => x.Id)
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(x => x.SkillName)
                .IsRequired(true)
                .HasColumnName("skill_name");

            builder.HasIndex(x => x.SkillName)
                .IsUnique();

            builder.Property(x => x.LevelCap)
                .IsRequired(true)
                .HasColumnName("level_cap");

            builder.HasMany<CoursesSkills>()
                .WithOne()
                .HasForeignKey(cs => cs.SkillId);

            builder.HasMany<JobsSkills>()
                .WithOne()
                .HasForeignKey(js => js.SkillId);
        }
    }
}
