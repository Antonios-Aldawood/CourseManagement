using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Enrollments.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Enrollments.Commands.AddEnrollmentAttendance
{
    public record AddEnrollmentAttendanceCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        int trainerId,
        int enrollmentId,
        int courseId,
        int sessionId,
        DateTimeOffset dateAttended) : IRequest<ErrorOr<AttendanceDto>>, IHeaderCarrier;
}
