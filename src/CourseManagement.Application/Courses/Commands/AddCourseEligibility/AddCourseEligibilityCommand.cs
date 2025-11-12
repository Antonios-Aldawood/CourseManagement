using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Courses.Commands.AddCourseEligibility
{
    public record AddCourseEligibilityCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        string courseSubject,
        string? position,
        List<int>? positionIds,
        string? department,
        List<int>? departmentIds,
        string? job,
        List<int>? jobIds) : IRequest<ErrorOr<List<EligibilityDto>>>, IHeaderCarrier;
}
