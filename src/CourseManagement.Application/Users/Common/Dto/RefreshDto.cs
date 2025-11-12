using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Users.Common.Dto
{
    public record RefreshDto : IHasAffectedIds
    {
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
        public required DateTime RefreshTokenExpiryDate { get; set; }

        public int AffectedId { get; set; }
    }
}
