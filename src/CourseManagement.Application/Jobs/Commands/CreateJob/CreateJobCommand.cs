using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Application.Jobs.Common.Dto;

namespace CourseManagement.Application.Jobs.Commands.CreateJob
{
    public record CreateJobCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        string title,
        double minSalary,
        double maxSalary,
        string description,
        string departmentName) : IRequest<ErrorOr<JobDto>>, IHeaderCarrier;
}
