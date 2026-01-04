using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Enrollments.Common.Dto;
using CourseManagement.Application.Common.Behaviors;
using CourseManagement.Application.Enrollments.Commands.Validator;
using CourseManagement.Domain.Enrollments;

namespace CourseManagement.Application.Enrollments.Commands.CreateOptionalEnrollment
{
    public class CreateOptionalEnrollmentCommandHandler(
        IEnrollmentsRepository enrollmentsRepository,
        IUsersRepository usersRepository,
        ICoursesRepository coursesRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<CreateOptionalEnrollmentCommand, ErrorOr<EnrollmentDto>>
    {
        private readonly IEnrollmentsRepository _enrollmentsRepository = enrollmentsRepository;
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly ICoursesRepository _coursesRepository = coursesRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<EnrollmentDto>> Handle(CreateOptionalEnrollmentCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _usersRepository.GetUserWithJobAndDepartmentByIdAsync(command.userId);
                if (user == null)
                {
                    return Error.Validation(description: "User not found.");
                }

                var authenticatedId = IdentityBehavior.CheckIfAuthenticationIdMatch(command.headers, command.userId);
                if (authenticatedId != Result.Success)
                {
                    return authenticatedId.Errors;
                }

                var course = await _coursesRepository.GetCourseByIdAsync(command.courseId);
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
                    return Error.Validation(description: "Enrollment for this user to this course already exists.");
                }

                int enrollmentsForCourseCount = await _enrollmentsRepository.GetEnrollmentsCountForCourse(course.Id);
                if (enrollmentsForCourseCount + 1 > (course.Sessions?.Sum(s => s.Seats) ?? int.MaxValue))
                {
                    return Error.Validation(description: "Course sessions seats insufficient.");
                }

                // Check if new course sessions conflict with sessions of already enrolled courses.
                var userCourses = await _coursesRepository.GetCoursesForUserAsync(command.userId);
                var userCoursesSessions = userCourses?
                    .Where(uC => uC.Sessions != null && uC.Sessions.Any())
                    .SelectMany(uC => uC.Sessions!)
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
                    isOptional: true);

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
            catch(Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}

/*
            if (course.Eligibilities is not null && course.IsForAll == false)
            {
                int positionId = User.ValidEnteredPosition(user.Position);

                bool positionMatch = course.Eligibilities.Any(e => e.Key == "Position" && e.Value == positionId);
                bool departmentMatch = course.Eligibilities.Any(e => e.Key == "Department" && e.Value == user.DepartmentId);
                bool jobMatch = course.Eligibilities.Any(e => e.Key == "Job" && e.Value == user.JobId);

                if (positionMatch == false &&
                    departmentMatch == false &&
                    jobMatch == false)
                {
                    return Error.Validation(description: "User not eligible to enroll in course.");
                }
}
*/