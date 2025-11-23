using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;

namespace CourseManagement.Application.Courses.Commands.AddCourseSessionMaterial
{
    public class AddCourseSessionMaterialCommandHandler(
        ICoursesRepository coursesRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<AddCourseSessionMaterialCommand, ErrorOr<SessionAndMaterialsDto>>
    {
        private readonly ICoursesRepository _coursesRepository = coursesRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<SessionAndMaterialsDto>> Handle(AddCourseSessionMaterialCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var course = await _coursesRepository.GetCourseWithSessionsAndSessionsMaterialsByCourseIdAsync(command.courseId);
                if (course == null)
                {
                    return Error.Validation(description: "Course not found.");
                }

                var material = course.AddCourseSessionMaterial(
                    sessionId: command.sessionId,
                    path: command.path,
                    isVideo: command.isVideo);

                if (material.IsError)
                {
                    return material.Errors;
                }

                await _unitOfWork.CommitChangesAsync();

                var session = course.Sessions!.First(s => s.Id == material.Value.SessionId);

                List<MaterialDto> materials = [];

                session.Materials!.ForEach(m =>
                    materials.Add(new MaterialDto
                    {
                        MaterialId = m.Id,
                        SessionId = m.SessionId,
                        SessionName = session.Name,
                        CourseId = session.CourseId,
                        CourseSubject = course.Subject,
                        Path = m.Path,
                        IsVideo = m.IsVideo,
                    }));

                return new SessionAndMaterialsDto
                {
                    SessionId = session.Id,
                    Name = session.Name,
                    CourseId = session.CourseId,
                    StartDate = session.StartDate,
                    EndDate = session.EndDate,
                    TrainerId = session.TrainerId,
                    IsOffline = session.IsOffline,
                    Seats = session.Seats,
                    Link = session.Link,
                    App = session.App,
                    Materials = materials,

                    //AffectedId = session.Value.Materials!.Last().Id
                    AffectedId = material.Value.Id
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
