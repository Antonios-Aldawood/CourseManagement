using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Users.Queries.FilterUsersByUpper2
{
    public record FilterUsersByUpper2Query(
        string ipAddress,
        Dictionary<string, string> headers,
        string upper2Alias) : IRequest<ErrorOr<List<UserDto>>>, IHeaderCarrier;
}
