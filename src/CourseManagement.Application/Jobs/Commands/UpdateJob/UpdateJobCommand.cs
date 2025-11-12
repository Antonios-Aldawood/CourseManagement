using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Application.Jobs.Common.Dto;

namespace CourseManagement.Application.Jobs.Commands.UpdateJob
{
    public record UpdateJobCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        string oldJobTitle,
        string? title,
        double? minSalary,
        double? maxSalary,
        string? description,
        int? departmentId) : IRequest<ErrorOr<JobDto>>, IHeaderCarrier;
}
