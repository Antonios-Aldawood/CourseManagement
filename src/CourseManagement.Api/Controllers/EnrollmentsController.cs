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
                        isCompleted: enrollment.IsCompleted)),
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
                        isCompleted: enrollment.IsCompleted)),
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
                            isCompleted: enrollment.IsCompleted))),
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
                            isCompleted: enrollment.IsCompleted))),
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
                            isCompleted: enrollment.IsCompleted))),
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
                            isCompleted: enrollment.IsCompleted))),
                Problem);
        }
    }
}
