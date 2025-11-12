using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Application.Jobs.Common.Dto;

namespace CourseManagement.Application.Jobs.Queries.FilterJobsBySalary
{
    public record FilterJobsBySalaryQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        double salary) : IRequest<ErrorOr<List<JobDto>>>, IHeaderCarrier;
}
