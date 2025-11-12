using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Courses.Commands.UpdateCourse
{
    public record UpdateCourseCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        string oldCourseSubject,
        string? subject,
        string? description) : IRequest<ErrorOr<CourseDto>>, IHeaderCarrier;
}
