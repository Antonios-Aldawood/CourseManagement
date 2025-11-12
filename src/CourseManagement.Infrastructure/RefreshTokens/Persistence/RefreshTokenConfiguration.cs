using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagement.Application.Users.Common.Token;
using CourseManagement.Domain.Users;

namespace CourseManagement.Infrastructure.RefreshTokens.Persistence
{
    internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(x => x.Id)
                .IsClustered(true);

            builder.Property(x => x.Id)
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            builder.Property(x => x.UserId)
                .IsRequired(true)
                .HasColumnName("user_id");

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);

            builder.Property(x => x.HashedRefreshToken)
                .IsRequired(true)
                .HasColumnName("hashed_refresh_token");

            builder.HasIndex(x => x.HashedRefreshToken)
                .IsUnique();

            builder.Property(x => x.CreatedAt)
                .IsRequired(true)
                .HasColumnName("created_at");

            builder.Property(x => x.ExpiresAt)
                .IsRequired(true)
                .HasColumnName("expires_at");

            builder.Property(x => x.RevokedAt)
                .IsRequired(false)
                .HasColumnName("revoked_at");

            builder.Property(x => x.ReplacedById)
                .IsRequired(false)
                .HasColumnName("replaced_by_id");

            builder.HasOne(x => x.ReplacedBy)
                .WithOne()
                .HasForeignKey<RefreshToken>(x => x.ReplacedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.RevokedReason)
                .IsRequired(false)
                .HasColumnName("revoked_reason");

            //builder.Property(x => x.IsActive)
            //    .IsRequired(true)
            //    .ValueGeneratedNever()
            //    .HasColumnName("is_active");
        }
    }
}
