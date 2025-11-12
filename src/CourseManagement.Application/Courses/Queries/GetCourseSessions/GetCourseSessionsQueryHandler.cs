using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;

namespace CourseManagement.Application.Courses.Queries.GetCourseSessions
{
    public class GetCourseSessionsQueryHandler(
        ICoursesRepository coursesRepository
        ) : IRequestHandler<GetCourseSessionsQuery, ErrorOr<List<CourseWithSessionsAndTrainersDto>>>
    {
        private readonly ICoursesRepository _coursesRepository = coursesRepository;

        public async Task<ErrorOr<List<CourseWithSessionsAndTrainersDto>>> Handle(GetCourseSessionsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var courseSessionsAndTrainers = await _coursesRepository.GetCourseWithSessionsAndTrainersBySubjectAsync(query.subject);
                if (courseSessionsAndTrainers is null ||
                    courseSessionsAndTrainers.Count == 0)
                {
                    return Error.Validation(description: "Course has no sessions or not found.");
                }

                return courseSessionsAndTrainers;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}

/*
                foreach (Session session in course.Sessions)
                {
                    SessionDto dto = new SessionDto
                    {
                        SessionId = session.Id,
                        CourseId = course.Id,
                        CourseSubject = course.Subject,
                        StartDate = session.StartDate,
                        EndDate = session.EndDate,
                        TrainerId = session.TrainerId,

                        AffectedId = session.Id
                    };

                    sessions.Add(dto);
                }

            try
            {
                if (await _coursesRepository.GetCourseWithSessionsBySubject(query.subject) is not Course course)
                {
                    return Error.Validation(description: "Course not found.");
                }

                List<SessionDto> sessions = [];
                if (course.Sessions == null ||
                    course.Sessions.Count == 0)
                {
                    return sessions;
                }

                sessions = course.Sessions.Select(s => new SessionDto
                {
                    SessionId = s.Id,
                    Name = s.Name,
                    CourseId = course.Id,
                    CourseSubject = course.Subject,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    TrainerId = s.TrainerId,

                    AffectedId = s.Id
                }).ToList();

                return sessions;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
*/