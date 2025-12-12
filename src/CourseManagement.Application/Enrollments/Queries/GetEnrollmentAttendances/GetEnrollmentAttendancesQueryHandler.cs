using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Enrollments.Common.Dto;

namespace CourseManagement.Application.Enrollments.Queries.GetEnrollmentAttendances
{
    public class GetEnrollmentAttendancesQueryHandler(
        IEnrollmentsRepository enrollmentsRepository
        ) : IRequestHandler<GetEnrollmentAttendancesQuery, ErrorOr<List<AttendanceDto>>>
    {
        private readonly IEnrollmentsRepository _enrollmentsRepository = enrollmentsRepository;

        public async Task<ErrorOr<List<AttendanceDto>>> Handle(GetEnrollmentAttendancesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var enrollment = await _enrollmentsRepository.GetEnrollmentWithUserAndCourseInfo(query.enrollmentId);
                if (enrollment == null)
                {
                    return Error.Validation(description: "Enrollment not found.");
                }

                return enrollment.First().Enrollment.Attendances?.Select(a => new AttendanceDto
                {
                    AttendanceId = a.Id,
                    EnrollmentId = a.EnrollmentId,
                    UserId = enrollment.First().UserId,
                    UserAlias = enrollment.First().UserAlias,
                    SessionId = a.SessionId,
                    SessionName = enrollment.Where(e => e.SessionId == a.SessionId).First().SessionName,
                    SessionTrainerId = enrollment.Where(e => e.SessionId == a.SessionId).First().SessionTrainerId,
                    SessionTrainerAlias = enrollment.Where(e => e.SessionId == a.SessionId).First().SessionTrainerAlias,
                    CourseId = enrollment.First().CourseId,
                    CourseSubject = enrollment.First().CourseSubject,
                    DateAttended = a.DateAttended,

                    AffectedId = a.Id,
                }).ToList() ?? [];
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
