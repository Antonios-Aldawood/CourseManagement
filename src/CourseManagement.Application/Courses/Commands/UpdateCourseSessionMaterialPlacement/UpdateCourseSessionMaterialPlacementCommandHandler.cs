using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Domain.Courses;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.Courses.Commands.UpdateCourseSessionMaterialPlacement
{
    public class UpdateCourseSessionMaterialPlacementCommandHandler(
        ICoursesRepository coursesRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateCourseSessionMaterialPlacementCommand, ErrorOr<SessionAndMaterialsDto>>
    {
        private readonly ICoursesRepository _coursesRepository = coursesRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<SessionAndMaterialsDto>> Handle(UpdateCourseSessionMaterialPlacementCommand command, CancellationToken cancellation)
        {
            try
            {
                var course = await _coursesRepository.GetCourseWithSessionsAndSessionsMaterialsByCourseIdAsync(command.oldCourseId);
                if (course == null)
                {
                    return Error.Validation(description: "Course not found.");
                }

                var newCourse = await _coursesRepository.GetCourseWithSessionsAndSessionsMaterialsByCourseIdAsync(command.newCourseId ?? 0);
                if (command.newCourseId != null &&
                    newCourse == null)
                {
                    return Error.Validation(description: "New course the material is being transferred to, does not exist.");
                }

                var updatedMaterial = course.UpdateCourseSessionMaterialSessionPlacement(
                    oldMaterialId: command.oldMaterialId,
                    oldSessionId: command.oldSessionId,
                    materialNewSessionName: command.newMaterialSessionName,
                    newCourse: newCourse);

                if (updatedMaterial.IsError)
                {
                    return updatedMaterial.Errors;
                }

                await _unitOfWork.CommitChangesAsync();

                var session = newCourse != null
                    ? newCourse.Sessions!.First(s => s.Id == updatedMaterial.Value.SessionId)
                    : course.Sessions!.First(s => s.Id == updatedMaterial.Value.SessionId);

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
                    AffectedId = updatedMaterial.Value.Id
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
