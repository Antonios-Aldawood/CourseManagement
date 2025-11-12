using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Roles
{
    public record RoleResponse(
        int RoleId,
        string RoleType);
}
