using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Domain.Users;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Users.Queries.GetUserProfile
{
    public record GetUserProfileQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        string alias) : IRequest<ErrorOr<User>>, IHeaderCarrier;
}
