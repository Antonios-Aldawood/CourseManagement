using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Application.Courses.Commands.Validator;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Domain.Courses;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.Courses.Commands.UpdateCourseSessionMaterial
{
    public class UpdateCourseSessionMaterialCommandHandler(
        ICoursesRepository coursesRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateCourseSessionMaterialCommand, ErrorOr<SessionAndMaterialsDto>>
    {
        private readonly ICoursesRepository _coursesRepository = coursesRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<SessionAndMaterialsDto>> Handle(UpdateCourseSessionMaterialCommand command, CancellationToken cancellationToken)
        {
            try
            {
                string? extension = Path.GetExtension(command.file.FileName).ToLowerInvariant();
                if (extension != ".mp4" &&
                    extension != ".pdf")
                {
                    return Error.Validation(description: "Only PDF and MP4 files are allowed.");
                }

                bool isFileFormatValidByBytes = await MaterialValidatorRules.ValidFileAsync(command.file, extension);
                if (isFileFormatValidByBytes == false)
                {
                    return Error.Validation(description: "File content does not match its extension.");
                }

                if (extension == ".mp4" && command.isVideo == false)
                {
                    return Error.Validation(description: "File is a video, but you entered it wasn't.");
                }

                var course = await _coursesRepository.GetCourseWithSessionsAndSessionsMaterialsByCourseIdAsync(command.courseId);
                if (course == null)
                {
                    return Error.Validation(description: "Course not found.");
                }

                var courseSession = course.CheckIfCourseHasSessionBySessionId(command.sessionId);
                if (courseSession.IsError)
                {
                    return courseSession.Errors;
                }

                if (courseSession.Value.Materials == null ||
                    courseSession.Value.Materials.Count == 0 ||
                    courseSession.Value.Materials.FirstOrDefault(m => m.Id == command.materialId) is not Material material)
                {
                    return Error.Validation(description: "Course session doesn't have material.");
                }

                var savedPath = await _coursesRepository.UpdateSavedCourseSessionMaterialAsync(course.Subject, courseSession.Value.Name, material.Path, command.file);

                var updatedMaterial = course.UpdateCourseSessionMaterial(
                    sessionId: command.sessionId,
                    materialId: command.materialId,
                    path: savedPath,
                    isVideo: command.isVideo);

                if (updatedMaterial.IsError)
                {
                    return updatedMaterial.Errors;
                }

                await _unitOfWork.CommitChangesAsync();

                var session = course.Sessions!.First(s => s.Id == updatedMaterial.Value.SessionId);

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
