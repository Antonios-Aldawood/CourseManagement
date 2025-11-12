using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Departments
{
    public record EditDepartmentRequest
    {
        public required string OldName { get; set; }
        public string? Name { get; set; }
        public int? MinMembers { get; set; }
        public int? MaxMembers { get; set; }
        public string? Description { get; set; }
    }
}
