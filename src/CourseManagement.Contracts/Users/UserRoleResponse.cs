using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Users
{
    public record UserRoleResponse(
        int Id,
        string Alias,
        string Role);
}
