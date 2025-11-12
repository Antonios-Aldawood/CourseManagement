using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagement.Domain.Users;
using CourseManagement.Domain.Roles;
using CourseManagement.Domain.Jobs;
using CourseManagement.Application.Logs.Entity;
using CourseManagement.Application.Users.Common.Token;
using CourseManagement.Domain.Enrollments;

namespace CourseManagement.Infrastructure.Users.Persistence
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id)
                .IsClustered(true);

            //builder.HasIndex(x => x.Id)
            //    .IsUnique();

            builder.Property(x => x.Id)
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(x => x.Alias)
                .IsRequired(true)
                .HasColumnName("alias");

            builder.HasIndex(x => x.Alias)
                .IsUnique();

            builder.Property(x => x.Email)
                .IsRequired(true)
                .HasColumnName("email");

            builder.HasIndex(x => x.Alias)
                .IsUnique();

            builder.Property(x => x.PasswordHash)
                .IsRequired(true)
                .HasColumnName("password_hash");

            builder.Property(x => x.PhoneNumber)
                .IsRequired(true)
                .HasColumnName("phone_number");

            builder.Property(x => x.Position)
                .IsRequired(true)
                .HasColumnName("position");

            builder.Property(x => x.RoleId)
                .IsRequired(true)
                .HasColumnName("role_id");

            builder.HasOne<Role>()
                .WithMany()
                .HasForeignKey(x => x.RoleId);

            builder.Property(x => x.AddressId)
                .IsRequired(true)
                .HasColumnName("address_id");

            builder.HasOne(x => x.Address)
                .WithMany()
                .HasForeignKey(x => x.AddressId);

            builder.Property(x => x.JobId)
                .IsRequired(true)
                .HasColumnName("job_id");

            builder.HasOne<Job>()
                .WithMany()
                .HasForeignKey(x => x.JobId);

            builder.Property(x => x.AgreedSalary)
                .IsRequired(true)
                .HasColumnName("agreed_salary");

            builder.Property(x => x.Upper1Id)
                .IsRequired(true)
                .HasColumnName("upper_1_id");

            builder.HasOne(x => x.Upper1)
                .WithMany()
                .HasForeignKey(x => x.Upper1Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Upper2Id)
                .IsRequired(true)
                .HasColumnName("upper_2_id");

            builder.HasOne(x => x.Upper2)
                .WithMany()
                .HasForeignKey(x => x.Upper2Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.RefreshToken)
                .IsRequired(true)
                .HasColumnName("refresh_token");

            builder.Property(x => x.RefreshTokenExpiryTime)
                .IsRequired(true)
                .HasColumnName("refresh_token_expiry_time");

            builder.Property(x => x.IsVerified)
                .IsRequired(true)
                .HasColumnName("is_verified");

            builder.Property(x => x.VerificationCode)
                .HasColumnName("verification_code");

            builder.HasMany<Log>()
                .WithOne()
                .HasForeignKey(l => l.UserId);

            builder.HasMany<RefreshToken>()
                .WithOne()
                .HasForeignKey(rT => rT.UserId);

            builder.HasMany<Enrollment>()
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
