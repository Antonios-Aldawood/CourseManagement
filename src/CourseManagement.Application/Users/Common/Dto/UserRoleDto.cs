using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Users.Common.Dto
{
    public record UserRoleDto : IHasAffectedIds
    {
        public required int Id { get; set; }
        public required string Alias { get; set; }
        public required string Role { get; set; }

        public int AffectedId { get; set; }
    }
}
