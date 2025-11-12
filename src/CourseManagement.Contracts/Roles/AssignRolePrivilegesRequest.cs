using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Roles
{
    public record AssignRolePrivilegesRequest
    {
        public required int RoleId { get; set; }
        public required List<int> PrivilegeIds { get; set; }
    }
}
