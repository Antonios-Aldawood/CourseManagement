using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagement.Domain.Courses;

namespace CourseManagement.Infrastructure.Materials.Persistence
{
    public class MaterialConfiguration : IEntityTypeConfiguration<Material>
    {
        public void Configure(EntityTypeBuilder<Material> builder)
        {
            builder.HasKey(x => x.Id)
                .IsClustered(true);

            builder.Property(x => x.Id)
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(x => x.Path)
                .IsRequired(true)
                .HasColumnName("path");

            builder.Property(x => x.IsVideo)
                .IsRequired(true)
                .HasColumnName("is_video");

            builder.Property(x => x.SessionId)
                .IsRequired(true)
                .HasColumnName("session_id");

            builder.HasOne<Session>()
                .WithMany()
                .HasForeignKey(x => x.SessionId);
        }
    }
}
