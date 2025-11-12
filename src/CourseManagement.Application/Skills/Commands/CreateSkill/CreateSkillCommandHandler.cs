using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Skills.Common.Dto;
using CourseManagement.Domain.Courses;
using CourseManagement.Domain.Skills;
using CourseManagement.Application.CourseSkill.Commands.CreateCourseSkill;

namespace CourseManagement.Application.Skills.Commands.CreateSkill
{
    public class CreateSkillCommandHandler(
        ISkillsRepository skillsRepository,
        ICoursesRepository coursesRepository,
        ISender mediator,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<CreateSkillCommand, ErrorOr<SkillDto>>
    {
        private readonly ISkillsRepository _skillsRepository = skillsRepository;
        private readonly ICoursesRepository _coursesRepository = coursesRepository;
        private readonly ISender _mediator = mediator;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<SkillDto>> Handle(CreateSkillCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (await _skillsRepository.SkillExistsAsync(command.skillName) == true)
                {
                    return Error.Validation(description: "Skill name already taken.");
                }

                if (await _coursesRepository.GetCourseByIdAsync(command.courseId) is not Course course)
                {
                    return Error.Validation(description: "Course does not exist.");
                }

                var skill = Skill.CreateSkill(
                    skillName: command.skillName,
                    levelCap: command.levelCap);

                if (skill.IsError)
                {
                    return skill.Errors;
                }

                await _skillsRepository.AddSkillAsync(skill.Value);

                await _unitOfWork.CommitChangesAsync();

                var courseSkillCommand = new CreateCourseSkillCommand(
                    ipAddress: command.ipAddress,
                    headers: command.headers,
                    courseId: course.Id,
                    skillId: skill.Value.Id,
                    weight: command.weight);

                var courseSkillResult = await _mediator.Send(courseSkillCommand);

                if (courseSkillResult.IsError)
                {
                    await _skillsRepository.DeleteSkillAsync(skill.Value.Id);
                    return courseSkillResult.Errors;
                }

                return new SkillDto
                {
                    SkillId = skill.Value.Id,
                    SkillName = skill.Value.SkillName,
                    LevelCap = skill.Value.LevelCap,
                    Course = course.Subject,
                    CourseSkillId = courseSkillResult.Value.CourseSkillId,
                    Weight = courseSkillResult.Value.Weight,

                    AffectedId = skill.Value.Id
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
