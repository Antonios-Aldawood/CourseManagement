using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Departments.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Departments.Commands.UpdateDepartment
{
    public record UpdateDepartmentCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        string oldDepartmentName,
        string? name,
        int? minMembers,
        int? maxMembers,
        string? description) : IRequest<ErrorOr<DepartmentDto>>, IHeaderCarrier;

}
