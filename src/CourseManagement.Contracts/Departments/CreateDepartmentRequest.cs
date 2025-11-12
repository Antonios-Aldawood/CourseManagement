using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Departments
{
    public record CreateDepartmentRequest
    {
        public required string Name { get; set; }
        public required int MinMembers { get; set; }
        public required int MaxMembers { get; set; }
        public required string Description { get; set; }
    }
}
