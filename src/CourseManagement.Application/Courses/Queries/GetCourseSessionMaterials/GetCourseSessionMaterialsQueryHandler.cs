using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;

namespace CourseManagement.Application.Courses.Queries.GetCourseSessionMaterials
{
    public class GetCourseSessionMaterialsQueryHandler(
        ICoursesRepository coursesRepository
        ) : IRequestHandler<GetCourseSessionMaterialsQuery, ErrorOr<List<MaterialDto>>>
    {
        private readonly ICoursesRepository _coursesRepository = coursesRepository;

        public async Task<ErrorOr<List<MaterialDto>>> Handle(GetCourseSessionMaterialsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var course = await _coursesRepository.GetCourseWithSessionsAndSessionsMaterialsByCourseIdAsync(query.courseId);
                if (course == null)
                {
                    return Error.Validation(description: "Course not found.");
                }

                var session = course.CheckIfCourseHasSessionBySessionId(query.sessionId);
                if (session.IsError)
                {
                    return session.Errors;
                }

                return session.Value.Materials != null ?
                    session.Value.Materials
                    .Select(m => new MaterialDto
                    {
                        MaterialId = m.Id,
                        SessionId = m.SessionId,
                        SessionName = session.Value.Name,
                        CourseId = session.Value.CourseId,
                        CourseSubject = course.Subject,
                        Path = m.Path,
                        IsVideo = m.IsVideo,

                        AffectedId = m.Id
                    }).ToList()
                    : [];
            }
            catch(Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
