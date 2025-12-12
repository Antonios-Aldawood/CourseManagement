using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Enrollments.Common.Dto;
using CourseManagement.Domain.Enrollments;

namespace CourseManagement.Application.Enrollments.Queries.GetEnrollmentAttendance
{
    public class GetEnrollmentAttendanceQueryHandler(
        IEnrollmentsRepository enrollmentsRepository
        ) : IRequestHandler<GetEnrollmentAttendanceQuery, ErrorOr<AttendanceDto>>
    {
        private readonly IEnrollmentsRepository _enrollmentsRepository = enrollmentsRepository;
        
        public async Task<ErrorOr<AttendanceDto>> Handle(GetEnrollmentAttendanceQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var enrollment = await _enrollmentsRepository.GetEnrollmentWithUserAndCourseInfo(query.enrollmentId);
                if (enrollment.Count == 0)
                {
                    return Error.Validation(description: "Enrollment not found.");
                }

                if (enrollment.First().Enrollment.Attendances?.FirstOrDefault(a => a.Id == query.attendanceId) is not Attendance attendance)
                {
                    return Error.Validation(description: "Attendance not found.");
                }

                return new AttendanceDto
                {
                    AttendanceId = attendance.Id,
                    EnrollmentId = attendance.EnrollmentId,
                    UserId = enrollment.First().UserId,
                    UserAlias = enrollment.First().UserAlias,
                    SessionId = attendance.SessionId,
                    SessionName = enrollment.Where(e => e.SessionId == attendance.SessionId).First().SessionName,
                    SessionTrainerId = enrollment.Where(e => e.SessionId == attendance.SessionId).First().SessionTrainerId,
                    SessionTrainerAlias = enrollment.Where(e => e.SessionId == attendance.SessionId).First().SessionTrainerAlias,
                    CourseId = enrollment.First().CourseId,
                    CourseSubject = enrollment.First().CourseSubject,
                    DateAttended = attendance.DateAttended,

                    AffectedId = attendance.Id,
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
