using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Departments
{
    public record DepartmentResponse(
        int DepartmentId,
        string Name,
        int MinMembers,
        int MaxMembers,
        string Description);
}
