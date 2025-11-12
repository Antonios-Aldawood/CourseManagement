using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Domain.Roles
{
    public class RolesPrivileges
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        public int PrivilegeId { get; set; }
        public Privilege? Privilege { get; set; }
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }

        public RolesPrivileges(
            int roleId,
            int privilegeId)
        {
            RoleId = roleId;
            PrivilegeId = privilegeId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
