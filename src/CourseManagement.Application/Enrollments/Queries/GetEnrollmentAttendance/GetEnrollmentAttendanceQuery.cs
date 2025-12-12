using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Enrollments.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Enrollments.Queries.GetEnrollmentAttendance
{
    public record GetEnrollmentAttendanceQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        int enrollmentId,
        int attendanceId) : IRequest<ErrorOr<AttendanceDto>>, IHeaderCarrier;
}
