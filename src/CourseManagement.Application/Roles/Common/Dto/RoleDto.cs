using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.Roles;

namespace CourseManagement.Application.Roles.Common.Dto
{
    public record RoleDto : IHasAffectedIds
    {
        public required int RoleId { get; set; }
        public required string RoleType { get; set; }

        public int AffectedId { get; set; }

        public static RoleDto AddDto(Role role)
        {
            RoleDto dto = new RoleDto
            {
                RoleId = role.Id,
                RoleType = role.RoleType
            };

            dto.AffectedId = role.Id;

            return dto;
        }
    }
}
