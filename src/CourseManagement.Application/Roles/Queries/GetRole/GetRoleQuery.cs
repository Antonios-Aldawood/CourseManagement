using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Roles.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Roles.Queries.GetRole
{
    public record GetRoleQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        string roleType) : IRequest<ErrorOr<List<RoleDto>>>, IHeaderCarrier;
}
