using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.Users.Common.Token
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string HashedRefreshToken { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset? RevokedAt { get; set; }
        public int? ReplacedById { get; set; }
        public RefreshToken? ReplacedBy { get; set; }
        public string? RevokedReason { get; set; }
        public bool IsActive => RevokedAt == null && DateTimeOffset.UtcNow < ExpiresAt;

        public RefreshToken(
            int userId,
            string hashedRefreshToken,
            DateTimeOffset expiresAt)
        {
            UserId = userId;
            HashedRefreshToken = hashedRefreshToken;
            ExpiresAt = expiresAt;
        }
    }
}
