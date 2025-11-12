using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Jobs.Common.Dto;

namespace CourseManagement.Application.Jobs.Queries.GetJobsByDepartment
{
    public record UnprivilegedGetJobsByDepartmentQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        string name) : IRequest<ErrorOr<List<JobDto>>>;
}
