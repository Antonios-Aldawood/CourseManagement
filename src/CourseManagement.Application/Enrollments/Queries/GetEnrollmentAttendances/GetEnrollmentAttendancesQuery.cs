using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Enrollments.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Enrollments.Queries.GetEnrollmentAttendances
{
    public record GetEnrollmentAttendancesQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        int enrollmentId) : IRequest<ErrorOr<List<AttendanceDto>>>, IHeaderCarrier;
}
