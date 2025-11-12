using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Users
{
    public record UserCreateRequest
    {
        public required string Alias { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Position { get; set; }
        public required string RoleType { get; set; }
        public required string City { get; set; }
        public required string Region { get; set; }
        public required string Road { get; set; }
        public required string JobTitle { get; set; }
        public required string Upper { get; set; }
        public required double AgreedSalary { get; set; }
    }
}
