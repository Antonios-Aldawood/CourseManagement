using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Enrollments.Common.Dto;
using CourseManagement.Application.Enrollments.Commands.Validator;
using CourseManagement.Domain.Enrollments;

namespace CourseManagement.Application.Enrollments.Commands.CreateForcedEnrollment
{
    public class CreateForcedEnrollmentCommandHandler(
        IEnrollmentsRepository enrollmentsRepository,
        IUsersRepository usersRepository,
        ICoursesRepository courseRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<CreateForcedEnrollmentCommand, ErrorOr<EnrollmentDto>>
    {
        private readonly IEnrollmentsRepository _enrollmentsRepository = enrollmentsRepository;
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly ICoursesRepository _coursesRepository = courseRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<EnrollmentDto>> Handle(CreateForcedEnrollmentCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _usersRepository.GetUserWithJobAndDepartmentByIdAsync(command.userId);
                if (user == null)
                {
                    return Error.Validation(description: "User not found.");
                }

                var course = await _coursesRepository.GetCourseWithSessionsByIdAsync(command.courseId);
                if (course == null)
                {
                    return Error.Validation(description: "Course not found.");
                }

                bool isEligible = EnrollmentEligibilityValidation.EnrollmentEligibilityValidator(user, course);
                if (course.IsForAll == false && isEligible == false)
                {
                    return Error.Validation(description: "User not eligible to enroll in course.");
                }

                bool existingEnrollment = await _enrollmentsRepository.ExistsForUserCourseAsync(command.userId, command.courseId);
                if (existingEnrollment)
                {
                    return Error.Validation(description: "Enrollment for this user to this course, already exists.");
                }

                // Check if new course sessions conflict with sessions of already enrolled courses.
                var userCourses = await _coursesRepository.GetCoursesForUserAsync(command.userId);
                var userCoursesSessions = userCourses?
                    .Where(uC => uC.Sessions != null && uC.Sessions.Any())
                    .SelectMany(uC=> uC.Sessions!)
                    .ToList() ?? [];

                if (course.Sessions?.Any() == true && userCoursesSessions.Any())
                {
                    bool hasConflict = userCoursesSessions.Any(uCS =>
                        uCS.EndDate >= DateTimeOffset.UtcNow &&
                        course.Sessions.Any(cS =>
                            !(cS.EndDate <= uCS.StartDate || cS.StartDate >= uCS.EndDate)));

                    if (hasConflict)
                    {
                        return Error.Validation(description: "User new course's sessions conflict with user's current course sessions.");
                    }
                }
                // Finished sessions conflict checking.

                var enrollment = Enrollment.CreateUserCourseEnrollment(
                    userId: user.UserId,
                    courseId: course.Id,
                    isOptional: false);

                if (enrollment.IsError)
                {
                    return enrollment.Errors;
                }

                await _enrollmentsRepository.AddEnrollmentAsync(enrollment.Value);

                await _unitOfWork.CommitChangesAsync();

                return new EnrollmentDto
                {
                    EnrollmentId = enrollment.Value.Id,
                    UserId = enrollment.Value.UserId,
                    UserAlias = user.Alias,
                    CourseId = enrollment.Value.CourseId,
                    CourseSubject = course.Subject,
                    IsOptional = enrollment.Value.IsOptional,
                    IsCompleted = enrollment.Value.IsCompleted,

                    AffectedId = enrollment.Value.Id
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}

/*
        // Checking new enrolled course session times conflict with sessions of prior enrolled courses for user.
using CourseManagement.Domain.Courses;
                List<Session> userCoursesSessions = [];

                var userCourses = await _coursesRepository.GetCoursesForUserAsync(command.userId);
                if (userCourses != null &&
                    userCourses.Count != 0)
                {
                    foreach (Course userCourse in userCourses)
                    {
                        if (userCourse.Sessions != null &&
                            userCourse.Sessions.Count != 0)
                        {
                            foreach (Session session in userCourse.Sessions)
                            {
                                userCoursesSessions.Add(session);
                            }
                        }
                    }
                }

                if (course.Sessions != null &&
                    course.Sessions.Count != 0 &&
                    userCoursesSessions != null &&
                    userCoursesSessions.Count != 0)
                {
                    bool isConflicting = userCoursesSessions.Any(uCS =>
                        course.Sessions.Any(cS =>
                             uCS.EndDate.CompareTo(DateTimeOffset.UtcNow) >= 0 &&
                             !(
                                (cS.StartDate.CompareTo(uCS.StartDate) <= 0 && cS.EndDate.CompareTo(uCS.StartDate) <= 0)
                                    ||
                                (cS.StartDate.CompareTo(uCS.EndDate) >= 0 && cS.EndDate.CompareTo(uCS.EndDate) >= 0)
                             )));

                    // Unencessary but working just the same foreach approach.
                    foreach (Session userCourseSession in userCoursesSessions)
                    {
                        foreach (Session session in course.Sessions)
                        {
                            if (
                                session.EndDate.CompareTo(DateTimeOffset.UtcNot) >= 0 &&
                                !(
                                (session.StartDate.CompareTo(userCourseSession.StartDate) <= 0 && session.EndDate.CompareTo(userCourseSession.StartDate) <= 0)
                                    ||
                                (session.StartDate.CompareTo(userCourseSession.EndDate) >= 0 && session.EndDate.CompareTo(userCourseSession.EndDate) >= 0)
                                ))
                            {
                                isConflicting = false;
                                break;
                            }
                        }
                    }
                    // Done with the unnecessary foreach approach.

                    if (isConflicting)
                    {
                        return Error.Validation(description: "User new course's sessions conflict with user current courses sessions.");
                    }
                }
        // Finished sessions conflict checking. 
*/
