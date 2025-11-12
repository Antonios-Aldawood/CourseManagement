using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Courses.Commands.CreateCourse
{
    public record CreateCourseCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        string subject,
        string description,
        string? position,
        List<int>? positionIds,
        string? department,
        List<int>? departmentIds,
        string? job,
        List<int>? jobIds,
        List<(string, int, int)> newSkills) : IRequest<ErrorOr<CourseDto>>, IHeaderCarrier;
}
