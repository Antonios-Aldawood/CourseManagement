using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Roles
{
    public record PrivilegeResponse(
        int PrivilegeId,
        string PrivilegeName);
}
