using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Courses.Queries.GetCourseSessions
{
    public record GetCourseSessionsQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        string subject) : IRequest<ErrorOr<List<CourseWithSessionsAndTrainersDto>>>, IHeaderCarrier;
}
