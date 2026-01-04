using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using CourseManagement.Contracts.Enrollments;
using CourseManagement.Application.Enrollments.Commands.CreateForcedEnrollment;
using CourseManagement.Application.Enrollments.Commands.CreateOptionalEnrollment;
using CourseManagement.Application.Enrollments.Queries.GetUserCourses;
using CourseManagement.Application.Enrollments.Queries.GetCourseUsers;
using CourseManagement.Application.Enrollments.Queries.GetOptionalEnrollmentsInEligibleCourses;
using CourseManagement.Application.Enrollments.Queries.GetForcedEnrollmentsInEligibleCourses;
using CourseManagement.Application.Enrollments.Queries.GetEnrollmentAttendances;
using CourseManagement.Application.Enrollments.Queries.GetEnrollmentAttendance;
using CourseManagement.Application.Enrollments.Commands.AddEnrollmentAttendance;
using CourseManagement.Application.Enrollments.Queries.GetEnrollmentsToConfirm;
using CourseManagement.Application.Enrollments.Commands.ConfirmOptionalEnrollment;

namespace CourseManagement.Api.Controllers
{
    [Route("[controller]")]
    public class EnrollmentsController(ISender _mediator) : ApiController
    {
        [Authorize]
        [HttpPost("Forced")]
        public async Task<IActionResult> AddForcedEnrollment(CreateEnrollmentRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new CreateForcedEnrollmentCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                userId: request.UserId,
                courseId: request.CourseId);

            var createForcedEnrollmentResult = await _mediator.Send(command);

            return createForcedEnrollmentResult.Match(
                enrollment => CreatedAtAction(
                    nameof(GetCoursesForUser),
                    new EnrollmentResponse(
                        enrollmentId: enrollment.EnrollmentId,
                        userId: enrollment.UserId,
                        userAlias: enrollment.UserAlias,
                        courseId: enrollment.CourseId,
                        courseSubject: enrollment.CourseSubject,
                        isOptional: enrollment.IsOptional,
                        isCompleted: enrollment.IsCompleted,
                        isConfirmed: enrollment.IsConfirmed)),
                Problem);
        }

        [Authorize]
        [HttpPost("Optional")]
        public async Task<IActionResult> AddOptionalEnrollment(CreateEnrollmentRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new CreateOptionalEnrollmentCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                userId: request.UserId,
                courseId: request.CourseId);

            var createForcedEnrollmentResult = await _mediator.Send(command);

            return createForcedEnrollmentResult.Match(
                enrollment => CreatedAtAction(
                    nameof(GetCoursesForUser),
                    new EnrollmentResponse(
                        enrollmentId: enrollment.EnrollmentId,
                        userId: enrollment.UserId,
                        userAlias: enrollment.UserAlias,
                        courseId: enrollment.CourseId,
                        courseSubject: enrollment.CourseSubject,
                        isOptional: enrollment.IsOptional,
                        isCompleted: enrollment.IsCompleted,
                        isConfirmed: enrollment.IsConfirmed)),
                Problem);
        }

        [Authorize]
        [HttpGet("User")]
        public async Task<IActionResult> GetCoursesForUser(int userId)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetUserCoursesQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                userId: userId);

            var getUserCoursesResult = await _mediator.Send(query);

            return getUserCoursesResult.Match(
                enrollments => Ok(
                    enrollments.ConvertAll(
                        enrollment => new EnrollmentResponse(
                            enrollmentId: enrollment.EnrollmentId,
                            userId: enrollment.UserId,
                            userAlias: enrollment.UserAlias,
                            courseId: enrollment.CourseId,
                            courseSubject: enrollment.CourseSubject,
                            isOptional: enrollment.IsOptional,
                            isCompleted: enrollment.IsCompleted,
                            isConfirmed: enrollment.IsConfirmed))),
                Problem);
        }

        [Authorize]
        [HttpGet("Course")]
        public async Task<IActionResult> GetUsersForCourse(int courseId)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetCourseUsersQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                courseId: courseId);

            var getUserCoursesResult = await _mediator.Send(query);

            return getUserCoursesResult.Match(
                enrollments => Ok(
                    enrollments.ConvertAll(
                        enrollment => new EnrollmentResponse(
                            enrollmentId: enrollment.EnrollmentId,
                            userId: enrollment.UserId,
                            userAlias: enrollment.UserAlias,
                            courseId: enrollment.CourseId,
                            courseSubject: enrollment.CourseSubject,
                            isOptional: enrollment.IsOptional,
                            isCompleted: enrollment.IsCompleted,
                            isConfirmed: enrollment.IsConfirmed))),
                Problem);
        }

        [Authorize]
        [HttpGet("User/Optional")]
        public async Task<IActionResult> GetOptionalEnrollmentsForUser(int userId)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetOptionalEnrollmentsInEligibleCoursesQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                userId: userId);

            var getOptionalEnrollmentsForUserResult = await _mediator.Send(query);

            return getOptionalEnrollmentsForUserResult.Match(
                enrollments => Ok(
                    enrollments.ConvertAll(
                        enrollment => new EnrollmentResponse(
                            enrollmentId: enrollment.EnrollmentId,
                            userId: enrollment.UserId,
                            userAlias: enrollment.UserAlias,
                            courseId: enrollment.CourseId,
                            courseSubject: enrollment.CourseSubject,
                            isOptional: enrollment.IsOptional,
                            isCompleted: enrollment.IsCompleted,
                            isConfirmed: enrollment.IsConfirmed))),
                Problem);
        }

        [Authorize]
        [HttpGet("User/Forced")]
        public async Task<IActionResult> GetForcedEnrollmentsForUser(int userId)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetForcedEnrollmentsInEligibleCoursesQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                userId: userId);

            var getForcedEnrollmentsForUserResult = await _mediator.Send(query);

            return getForcedEnrollmentsForUserResult.Match(
                enrollments => Ok(
                    enrollments.ConvertAll(
                        enrollment => new EnrollmentResponse(
                            enrollmentId: enrollment.EnrollmentId,
                            userId: enrollment.UserId,
                            userAlias: enrollment.UserAlias,
                            courseId: enrollment.CourseId,
                            courseSubject: enrollment.CourseSubject,
                            isOptional: enrollment.IsOptional,
                            isCompleted: enrollment.IsCompleted,
                            isConfirmed: enrollment.IsConfirmed))),
                Problem);
        }

        [Authorize]
        [HttpPost("Attendance")]
        public async Task<IActionResult> AddEnrollmentAttendance(AddEnrollmentAttendanceRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new AddEnrollmentAttendanceCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                trainerId: request.TrainerId,
                enrollmentId: request.EnrollmentId,
                courseId: request.CourseId,
                sessionId: request.SessionId,
                dateAttended: request.DateAttended);

            var addEnrollmentAttendanceResult = await _mediator.Send(command);

            return addEnrollmentAttendanceResult.Match(
                attendance => CreatedAtAction(
                    nameof(GetEnrollmentAttendance),
                    new AttendanceResponse(
                        AttendanceId: attendance.AttendanceId,
                        EnrollmentId: attendance.EnrollmentId,
                        UserId: attendance.UserId,
                        UserAlias: attendance.UserAlias,
                        SessionId: attendance.SessionId,
                        SessionName: attendance.SessionName,
                        SessionTrainerId: attendance.SessionTrainerId,
                        SessionTrainerAlias: attendance.SessionTrainerAlias,
                        CourseId: attendance.CourseId,
                        CourseSubject: attendance.CourseSubject,
                        DateAttended: attendance.DateAttended)),
                Problem);
        }

        [Authorize]
        [HttpGet("Enrollment/Attendances")]
        public async Task<IActionResult> GetEnrollmentAttendances(int EnrollmentId)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetEnrollmentAttendancesQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                enrollmentId: EnrollmentId);

            var getEnrollmentAttendancesResult = await _mediator.Send(query);

            return getEnrollmentAttendancesResult.Match(
                attendances => Ok(
                    attendances.ConvertAll(
                        attendance => new AttendanceResponse(
                            AttendanceId: attendance.AttendanceId,
                            EnrollmentId: attendance.EnrollmentId,
                            UserId: attendance.UserId,
                            UserAlias: attendance.UserAlias,
                            SessionId: attendance.SessionId,
                            SessionName: attendance.SessionName,
                            SessionTrainerId: attendance.SessionTrainerId,
                            SessionTrainerAlias: attendance.SessionTrainerAlias,
                            CourseId: attendance.CourseId,
                            CourseSubject: attendance.CourseSubject,
                            DateAttended: attendance.DateAttended))),
                Problem);
        }

        [Authorize]
        [HttpGet("Enrollment/Attendance")]
        public async Task<IActionResult> GetEnrollmentAttendance([FromQuery] int EnrollmentId, [FromQuery] int AttendanceId)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetEnrollmentAttendanceQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                enrollmentId: EnrollmentId,
                attendanceId: AttendanceId);

            var getEnrollmentAttendanceResult = await _mediator.Send(query);

            return getEnrollmentAttendanceResult.Match(
                attendance => Ok(new AttendanceResponse(
                    AttendanceId: attendance.AttendanceId,
                    EnrollmentId: attendance.EnrollmentId,
                    UserId: attendance.UserId,
                    UserAlias: attendance.UserAlias,
                    SessionId: attendance.SessionId,
                    SessionName: attendance.SessionName,
                    SessionTrainerId: attendance.SessionTrainerId,
                    SessionTrainerAlias: attendance.SessionTrainerAlias,
                    CourseId: attendance.CourseId,
                    CourseSubject: attendance.CourseSubject,
                    DateAttended: attendance.DateAttended)),
                Problem);
        }

        [Authorize]
        [HttpGet("ToBeConfirmed")]
        public async Task<IActionResult> GetEnrollmentsToBeConfirmed()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetEnrollmentsToConfirmQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var getEnrollmentsToBeConfirmedResult = await _mediator.Send(query);

            return getEnrollmentsToBeConfirmedResult.Match(
                enrollments => Ok(
                    enrollments.ConvertAll(
                        enrollment => new EnrollmentResponse(
                            enrollmentId: enrollment.EnrollmentId,
                            userId: enrollment.UserId,
                            userAlias: enrollment.UserAlias,
                            courseId: enrollment.CourseId,
                            courseSubject: enrollment.CourseSubject,
                            isOptional: enrollment.IsOptional,
                            isCompleted: enrollment.IsCompleted,
                            isConfirmed: enrollment.IsConfirmed))),
                Problem);
        }

        [Authorize]
        [HttpPut("Confirm")]
        public async Task<IActionResult> ConfirmOptionalEnrollment(ConfirmEnrollmentRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new ConfirmOptionalEnrollmentCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                enrollmentId: request.EnrollmentId);

            var confirmOptionalEnrollmentResult = await _mediator.Send(command);

            return confirmOptionalEnrollmentResult.Match(
                enrollment => CreatedAtAction(
                    nameof(ConfirmOptionalEnrollment),
                    new EnrollmentResponse(
                            enrollmentId: enrollment.EnrollmentId,
                            userId: enrollment.UserId,
                            userAlias: enrollment.UserAlias,
                            courseId: enrollment.CourseId,
                            courseSubject: enrollment.CourseSubject,
                            isOptional: enrollment.IsOptional,
                            isCompleted: enrollment.IsCompleted,
                            isConfirmed: enrollment.IsConfirmed)),
                Problem);
        }
    }
}
