using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagement.Domain.Roles;

namespace CourseManagement.Infrastructure.RolePrivilege.Persistence
{
    public class RolesPrivilegesConfiguration : IEntityTypeConfiguration<RolesPrivileges>
    {
        public void Configure(EntityTypeBuilder<RolesPrivileges> builder)
        {
            builder.HasKey(rp => rp.Id)
                .IsClustered(true);

            builder.Property(rp => rp.Id)
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(rp => rp.RoleId)
                .IsRequired(true)
                .HasColumnName("role_id");

            builder.HasOne(rp => rp.Role)
                .WithMany(r => r.RolesPrivileges)
                .HasForeignKey(rp => rp.RoleId);

            builder.Property(rp => rp.PrivilegeId)
                .IsRequired(true)
                .HasColumnName("privilege_id");

            builder.HasOne(rp => rp.Privilege)
                .WithMany()
                .HasForeignKey(rp => rp.PrivilegeId);

            builder.Property(rp => rp.CreatedAt)
                .IsRequired(true)
                .HasColumnName("created_at");

            builder.Property(rp => rp.UpdatedAt)
                .IsRequired(true)
                .HasColumnName("updated_at");
        }
    }
}
