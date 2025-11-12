using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagement.Domain.JobsSkills;
using CourseManagement.Domain.Jobs;
using CourseManagement.Domain.Skills;

namespace CourseManagement.Infrastructure.JobSkill.Persistence
{
    public class JobsSkillsConfiguration : IEntityTypeConfiguration<JobsSkills>
    {
        public void Configure(EntityTypeBuilder<JobsSkills> builder)
        {
            builder.HasKey(x => x.Id)
                .IsClustered(true);

            builder.Property(x => x.Id)
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(x => x.JobId)
                .IsRequired(true)
                .HasColumnName("job_id");

            builder.HasOne<Job>()
                .WithMany()
                .HasForeignKey(x => x.JobId);

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
