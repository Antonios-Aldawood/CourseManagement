using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Roles.Common.Dto
{
    public record PrivilegeDto : IHasAffectedIds
    {
        public required int PrivilegeId { get; set; }
        public required string PrivilegeName { get; set; }

        public int AffectedId { get; set; }
    }
}
