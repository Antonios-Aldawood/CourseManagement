using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Users
{
    public record UserAddPrivilege
    {
        public required int UserId { get; set; }
        public required string NewRole { get; set; }
        public required List<int> PrivilegeIds { get; set; }
    }
}
