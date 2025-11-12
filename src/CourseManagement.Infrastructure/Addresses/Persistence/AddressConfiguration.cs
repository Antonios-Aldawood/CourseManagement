using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagement.Domain.Users;

namespace CourseManagement.Infrastructure.Addresses.Persistence
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(x => x.Id)
                .IsClustered(true);

            builder.Property(x => x.Id)
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(x => x.City)
                .IsRequired(true)
                .HasColumnName("city");

            builder.Property(x => x.Region)
                .IsRequired(true)
                .HasColumnName("region");

            builder.Property(x => x.Road)
                .IsRequired(true)
                .HasColumnName("road");

            builder.HasMany<User>()
                .WithOne(u => u.Address)
                .HasForeignKey(u => u.AddressId);
        }
    }
}
