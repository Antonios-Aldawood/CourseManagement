using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.Users.Common.Dto
{
    public class UserFullDto
    {
        public required int Id { get; set; }
        public required string Alias { get; set; } = string.Empty;
        public required string Email { get; set; } = string.Empty;
        public required string PasswordHash { get; set; } = string.Empty;
        public required string PhoneNumber { get; set; } = string.Empty;
        public required string Position { get; set; } = string.Empty;
        public required string RoleType { get; set; } = string.Empty;
        public required string City { get; set; } = string.Empty;
        public required string Region { get; set; } = string.Empty;
        public required string Road { get; set; } = string.Empty;
        public required string JobTitle { get; set; } = string.Empty;
        public required string JobDescription { get; set; } = string.Empty;
        public required string DepartmentName { get; set; } = string.Empty;
        public required string Upper1 { get; set; } = string.Empty;
        public required string Upper2 { get; set; } = string.Empty;
        public required string RefreshToken { get; set; } = string.Empty;
        public required bool IsVerified { get; set; }
    }
}
