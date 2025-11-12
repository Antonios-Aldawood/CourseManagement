using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;

namespace CourseManagement.Application.Courses.Commands.UpdateCourse
{
    public class UpdateCourseCommandHandler(
        ICoursesRepository coursesRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateCourseCommand, ErrorOr<CourseDto>>
    {
        private readonly ICoursesRepository _coursesRepository = coursesRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<CourseDto>> Handle(UpdateCourseCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var oldCourse = await _coursesRepository.GetCourseByExactSubjectAsync(command.oldCourseSubject);

                if (oldCourse == null)
                {
                    return Error.Validation(description: "Course does not exist.");
                }

                if (command.subject != null &&
                    command.subject != oldCourse.Subject &&
                    await _coursesRepository.SubjectExistsAsync(command.subject) == true)
                {
                    return Error.Validation(description: "Course subject already taken.");
                }

                var newCourse = oldCourse.UpdateCourse(
                    subject: command.subject ?? oldCourse.Subject,
                    description: command.description ?? oldCourse.Description);

                _coursesRepository.UpdateCourse(oldCourse, newCourse);

                await _unitOfWork.CommitChangesAsync();

                return CourseDto.AddCourseDto(newCourse);
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
