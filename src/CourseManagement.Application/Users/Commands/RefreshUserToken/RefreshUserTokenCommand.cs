using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Users.Commands.RefreshUserToken
{
    public record RefreshUserTokenCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        string refreshToken) : IRequest<ErrorOr<RefreshDto>>, IHeaderCarrier;
}
