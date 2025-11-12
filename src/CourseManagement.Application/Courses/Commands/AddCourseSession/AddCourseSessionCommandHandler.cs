using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;

namespace CourseManagement.Application.Courses.Commands.AddCourseSession
{
    public class AddCourseSessionCommandHandler(
        ICoursesRepository courseRepository,
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<AddCourseSessionCommand, ErrorOr<SessionWithTrainerDto>>
    {
        private readonly ICoursesRepository _courseRepository = courseRepository;
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<SessionWithTrainerDto>> Handle(AddCourseSessionCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var course = await _courseRepository.GetCourseWithSessionsBySubjectAsync(command.subject);
                if (course == null)
                {
                    return Error.Validation(description: "Course not found.");
                }

                var trainer = await _usersRepository.GetTrainerByIdAsync(command.trainerId);
                if (trainer is null)
                {
                    return Error.Validation(description: "Trainer not found.");
                }

                var coursesAndSessionsForUsersEnrolledInThisCourse = await _courseRepository.GetCoursesWithSessionsForUsersEnrolledInThisCourse(course.Id);
                bool hasConflict = coursesAndSessionsForUsersEnrolledInThisCourse.Any(c =>
                    c.Sessions != null &&
                    c.Sessions.Any(s =>
                        s.EndDate >= DateTimeOffset.UtcNow &&
                        !(command.endDate <= s.StartDate || command.startDate >= s.EndDate)));
                if (hasConflict)
                {
                    return Error.Validation(description: "Conflict with other session start and end dates in courses for users already enrolled in this course.");
                }

                var session = course.AddCourseSession(
                    sessionName: command.sessionName,
                    startDate: command.startDate,
                    endDate: command.endDate,
                    trainerId: command.trainerId,
                    isOffline: command.isOffline,
                    seats: command.seats,
                    link: command.link,
                    app: command.app);

                if (session.IsError)
                {
                    return session.Errors;
                }

                await _unitOfWork.CommitChangesAsync();

                return new SessionWithTrainerDto
                {
                    SessionId = session.Value.Id,
                    Name = session.Value.Name,
                    CourseId = course.Id,
                    CourseSubject = course.Subject,
                    StartDate = session.Value.StartDate,
                    EndDate = session.Value.EndDate,
                    TrainerId = session.Value.TrainerId,
                    TrainerAlias = trainer.Alias,
                    IsOffline = session.Value.IsOffline,
                    Seats = session.Value.Seats,
                    Link = session.Value.Link,
                    App = session.Value.App,

                    AffectedId = session.Value.Id
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
