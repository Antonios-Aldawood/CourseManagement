using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Courses.Queries.GetCoursesForEligibilities
{
    public record GetCoursesForEligibilitiesQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        int userId) : IRequest<ErrorOr<List<CourseDto>>>, IHeaderCarrier;
}
