using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Domain.Courses;
using CourseManagement.Application.Courses.Commands.AddCourseEligibility;
using CourseManagement.Application.Skills.Commands.CreateSkill;
using CourseManagement.Application.CourseSkill.Commands.CreateCourseSkill;
using CourseManagement.Domain.Skills;

namespace CourseManagement.Application.Courses.Commands.CreateCourse
{
    public class CreateCourseCommandHandler(
        ICoursesRepository coursesRepository,
        ISkillsRepository skillsRepository,
        ISender mediator,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<CreateCourseCommand, ErrorOr<CourseDto>>
    {
        private readonly ICoursesRepository _coursesRepository = coursesRepository;
        private readonly ISkillsRepository _skillsRepository = skillsRepository;
        private readonly ISender _mediator = mediator;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<CourseDto>> Handle(CreateCourseCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (await _coursesRepository.SubjectExistsAsync(command.subject) == true)
                {
                    return Error.Validation(description: "Course subject already taken.");
                }

                if (command.newSkills.Count > 3)
                {
                    return Error.Validation(description: "Can not give a course more than 3 skills.");
                }

                var course = Course.CreateCourse(
                    subject: command.subject,
                    description: command.description);

                if (course.IsError)
                {
                    return course.Errors;
                }

                await _coursesRepository.AddCourseAsync(course.Value);
                
                await _unitOfWork.CommitChangesAsync();

                CourseDto dto = CourseDto.AddCourseDto(course.Value);

                var eligibility = new AddCourseEligibilityCommand(
                    ipAddress: command.ipAddress,
                    headers: command.headers,
                    courseSubject: course.Value.Subject,
                    position: command.position,
                    positionIds: command.positionIds,
                    department: command.department,
                    departmentIds: command.departmentIds,
                    job: command.job,
                    jobIds: command.jobIds);

                var eligibilityResult = await _mediator.Send(eligibility);

                if (eligibilityResult.IsError)
                {
                    await _coursesRepository.DeleteCourseAsync(course.Value.Subject);

                    return eligibilityResult.Errors;
                }

                foreach (var enteredSkill in command.newSkills)
                {
                    var skill = new CreateSkillCommand(
                    ipAddress: command.ipAddress,
                    headers: command.headers,
                    skillName: enteredSkill.Item1,
                    levelCap: enteredSkill.Item2,
                    courseId: course.Value.Id,
                    weight: enteredSkill.Item3);

                    var skillResult = await _mediator.Send(skill);

                    if (skillResult.Errors.Any(e => e.Description == "Skill name already taken."))
                    {
                        Skill? existingSkill = await _skillsRepository.GetSkillByExactSkillNameAsync(enteredSkill.Item1);

                        var courseSkillCommand = new CreateCourseSkillCommand(
                        ipAddress: command.ipAddress,
                        headers: command.headers,
                        courseId: course.Value.Id,
                        skillId: existingSkill!.Id,
                        weight: enteredSkill.Item3);

                        var courseSkillResult = await _mediator.Send(courseSkillCommand);

                        if (courseSkillResult.IsError)
                        {
                            return courseSkillResult.Errors;
                        }
                    }

                    if (skillResult.IsError &&
                        skillResult.Errors.Any(e => e.Description == "Skill name already taken.") == false)
                    {
                        await _coursesRepository.DeleteCourseAsync(course.Value.Subject);

                        return skillResult.Errors;
                    }
                }

                return dto;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
