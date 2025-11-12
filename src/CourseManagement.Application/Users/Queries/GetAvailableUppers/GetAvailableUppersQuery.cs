using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Users.Queries.GetAvailableUppers
{
    public record GetAvailableUppersQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        string jobTitle) : IRequest<ErrorOr<List<string>>>, IHeaderCarrier;
}
