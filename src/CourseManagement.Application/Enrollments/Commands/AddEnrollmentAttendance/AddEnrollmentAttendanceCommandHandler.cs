using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Enrollments.Common.Dto;
using CourseManagement.Application.Common.Behaviors;

namespace CourseManagement.Application.Enrollments.Commands.AddEnrollmentAttendance
{
    public class AddEnrollmentAttendanceCommandHandler(
        IEnrollmentsRepository enrollmentsRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<AddEnrollmentAttendanceCommand, ErrorOr<AttendanceDto>>
    {
        private readonly IEnrollmentsRepository _enrollmentsRepository = enrollmentsRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<AttendanceDto>> Handle(AddEnrollmentAttendanceCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var authenticatedId = IdentityBehavior.CheckIfAuthenticationIdMatch(command.headers, command.trainerId);
                if (authenticatedId.IsError)
                {
                    return authenticatedId.Errors;
                }

                var enrollment = await _enrollmentsRepository.GetEnrollmentWithUserAndCourseInfo(command.enrollmentId);
                if (enrollment.Count == 0)
                {
                    return Error.Validation(description: "Enrollment not found.");
                }

                if (enrollment.First().Enrollment.IsConfirmed == false)
                {
                    return Error.Validation(description: "Can't attend this course's sessions because enrollment hasn't been confirmed.");
                }

                var courseInfo = enrollment.FirstOrDefault(e => e.CourseId == command.courseId);
                if (courseInfo == null)
                {
                    return Error.Validation(description: "Course not found for enrollment.");
                }

                var sessionInfo = enrollment
                    .Where(e => e.CourseId == courseInfo.CourseId && e.SessionId == command.sessionId)
                    .FirstOrDefault();
                if (sessionInfo == null)
                {
                    return Error.Validation(description: "Session not found in enrolled course.");
                }

                if (command.dateAttended < sessionInfo.StartDate ||
                    command.dateAttended > sessionInfo.EndDate)
                {
                    return Error.Validation(description: "Attendance date can not be outside session start and end date.");
                }

                if (sessionInfo.SessionTrainerId != command.trainerId)
                {
                    return Error.Validation(description: "Trainer confirming attendance is not session trainer.");
                }

                var attendance = enrollment.First().Enrollment.AddEnrollmentAttendance(
                    sessionId: sessionInfo.SessionId,
                    dateAttended: command.dateAttended);

                if (attendance.IsError)
                {
                    return attendance.Errors;
                }

                await _unitOfWork.CommitChangesAsync();

                return new AttendanceDto
                {
                    AttendanceId = attendance.Value.Id,
                    EnrollmentId = attendance.Value.EnrollmentId,
                    UserId = enrollment.First().UserId,
                    UserAlias = enrollment.First().UserAlias,
                    SessionId = attendance.Value.SessionId,
                    SessionName = sessionInfo.SessionName,
                    SessionTrainerId = sessionInfo.SessionTrainerId,
                    SessionTrainerAlias = sessionInfo.SessionTrainerAlias,
                    CourseId = courseInfo.CourseId,
                    CourseSubject = courseInfo.CourseSubject,
                    DateAttended = attendance.Value.DateAttended,

                    AffectedId = attendance.Value.Id
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
