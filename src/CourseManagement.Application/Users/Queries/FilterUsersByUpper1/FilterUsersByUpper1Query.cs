using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Users.Queries.FilterUsersByUpper1
{
    public record FilterUsersByUpper1Query(
        string ipAddress,
        Dictionary<string, string> headers,
        string upper1Alias) : IRequest<ErrorOr<List<UserDto>>>, IHeaderCarrier;
}
