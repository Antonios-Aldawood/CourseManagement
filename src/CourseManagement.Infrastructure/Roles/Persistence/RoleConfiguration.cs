using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagement.Domain.Roles;

namespace CourseManagement.Infrastructure.Roles.Persistence
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(x => x.Id)
                .IsClustered(true);

            builder.Property(x => x.Id)
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(x => x.RoleType)
                .IsRequired(true)
                .HasColumnName("role_type");

            builder.HasIndex(x => x.RoleType)
                .IsUnique();

            builder.HasMany(x => x.RolesPrivileges)
                .WithOne(rp => rp.Role)
                .HasForeignKey(rp => rp.RoleId);
        }
    }
}
