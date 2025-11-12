using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Users.Commands.CreateUser
{
    public record CreateUserCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        string alias,
        string email,
        string password,
        string phoneNumber,
        string position,
        string roleType,
        string city,
        string region,
        string road,
        string title,
        string upper,
        double agreedSalary) : IRequest<ErrorOr<UserShortDto>>, IHeaderCarrier;
}
