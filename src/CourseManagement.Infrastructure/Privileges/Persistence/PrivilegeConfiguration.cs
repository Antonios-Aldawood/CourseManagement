using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagement.Domain.Roles;

namespace CourseManagement.Infrastructure.Privileges.Persistence
{
    public class PrivilegeConfiguration : IEntityTypeConfiguration<Privilege>
    {
        public void Configure(EntityTypeBuilder<Privilege> builder)
        {
            builder.HasKey(x => x.Id)
                .IsClustered(true);

            builder.Property(x => x.Id)
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(x => x.PrivilegeName)
                .IsRequired(true)
                .HasColumnName("privilege_name");

            builder.HasIndex(x => x.PrivilegeName)
                .IsUnique();

            builder.HasMany<RolesPrivileges>()
                .WithOne(rp => rp.Privilege)
                .HasForeignKey(rp => rp.PrivilegeId);
        }
    }
}
