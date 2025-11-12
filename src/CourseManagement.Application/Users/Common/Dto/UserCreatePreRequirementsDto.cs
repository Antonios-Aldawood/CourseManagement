using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Domain.Roles;
using CourseManagement.Domain.Jobs;
using CourseManagement.Domain.Users;

namespace CourseManagement.Application.Users.Common.Dto
{
    public record UserCreatePreRequirementsDto
    {
        public int Id { get; set; }
        public string? Alias { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public Role? Role { get; set; } = null;
        public int JobId { get; set; }
        public Job? Job { get; set; } = null;
        public string? DepartmentName { get; set; } = string.Empty;
        public int AddressId { get; set; }
        public Address? Address { get; set; } = null;
    }
}
