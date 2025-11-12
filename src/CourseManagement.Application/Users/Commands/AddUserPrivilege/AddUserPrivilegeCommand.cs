using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Users.Commands.AddUserPrivilege
{
    public record AddUserPrivilegeCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        int userId,
        string newRole,
        List<int> privilegeIds) : IRequest<ErrorOr<UserRoleDto>>, IHeaderCarrier;
}
