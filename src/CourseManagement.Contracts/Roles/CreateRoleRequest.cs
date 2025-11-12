using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Roles
{
    public record CreateRoleRequest
    {
        public required string RoleType { get; set; }
    }
}
