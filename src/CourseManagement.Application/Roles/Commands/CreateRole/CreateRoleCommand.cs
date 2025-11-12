using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Roles.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Roles.Commands.CreateRole
{
    public record CreateRoleCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        string roleType) : IRequest<ErrorOr<RoleDto>>, IHeaderCarrier;
}

/*
    public record CreateRoleCommand(
        string roleType,
        List<int> privilegeIds) : IRequest<ErrorOr<Role>>;
*/
