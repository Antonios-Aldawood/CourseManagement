using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Roles.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Roles.Commands.AddRolePrivileges
{
    public record AddRolePrivilegesCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        int roleId,
        List<int> privilegeIds) : IRequest<ErrorOr<RoleDto>>, IHeaderCarrier;
}
