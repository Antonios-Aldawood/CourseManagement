using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Roles.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Users.Queries.GetPrivilegesByUser
{
    public record GetPrivilegesByUserQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        string alias) : IRequest<ErrorOr<List<PrivilegeDto>>>, IHeaderCarrier;
}
