using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Application.Jobs.Common.Dto;

namespace CourseManagement.Application.Jobs.Queries.GetJob
{
    public record GetJobQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        string title) : IRequest<ErrorOr<List<JobDto>>>, IHeaderCarrier;
}
