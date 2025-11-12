using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Common.Dto;
using CourseManagement.Application.Users.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Users.Queries.GetAllUsers
{
    public record PaginatedGetAllUsersQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        int pageNumber = 1,
        int pageSize = 10) : IRequest<ErrorOr<PagedResult<UserDto>>>, IHeaderCarrier;
}
