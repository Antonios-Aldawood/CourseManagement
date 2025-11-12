using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Users.Common.Dto
{
    public record UserDto : IHasAffectedIds
    {
        public required int UserId { get; set; }
        public required string Alias { get; set; } = string.Empty;
        public required string Email { get; set; } = string.Empty;
        public required string PhoneNumber { get; set; } = string.Empty;
        public required string Position { get; set; } = string.Empty;
        public required string City { get; set; } = string.Empty;
        public required string Region { get; set; } = string.Empty;
        public required string Road { get; set; } = string.Empty;
        public required string Role { get; set; } = string.Empty;
        public required string JobTitle { get; set; } = string.Empty;
        public required string JobDescription { get; set; } = string.Empty;
        public required string DepartmentName { get; set; } = string.Empty;
        public required string Upper1Alias { get; set; } = string.Empty;
        public required string Upper2Alias { get; set; } = string.Empty;
        public required bool IsVerified { get; set; }

        public int AffectedId { get; set; }
    }
}
