using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Application.Common.Dto;

namespace CourseManagement.Application.Courses.Queries.GetAllCourses
{
    public record GetAllCoursesQuery(
        string ipAddress,
        Dictionary<string, string> headers) : IRequest<ErrorOr<List<CourseDto>>>, IHeaderCarrier;
}
