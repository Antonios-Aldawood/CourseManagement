using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Users.Queries.FilterUsersByRole
{
    public record FilterUsersByRoleQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        string role) : IRequest<ErrorOr<List<UserDto>>>, IHeaderCarrier;
}
