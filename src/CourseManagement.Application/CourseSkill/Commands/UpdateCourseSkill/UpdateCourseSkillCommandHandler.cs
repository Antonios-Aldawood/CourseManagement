using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.CourseSkill.Common.Dto;
using CourseManagement.Domain.Courses;
using CourseManagement.Domain.Skills;

namespace CourseManagement.Application.CourseSkill.Commands.UpdateCourseSkill
{
    public class UpdateCourseSkillCommandHandler(
        ICoursesSkillsRepository coursesSkillsRepository,
        ICoursesRepository coursesRepository,
        ISkillsRepository skillsRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateCourseSkillCommand, ErrorOr<CoursesSkillsDto>>
    {
        private readonly ICoursesSkillsRepository _coursesSkillsRepository = coursesSkillsRepository;
        private readonly ICoursesRepository _coursesRepository = coursesRepository;
        private readonly ISkillsRepository _skillsRepository = skillsRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<CoursesSkillsDto>> Handle(UpdateCourseSkillCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var oldCourseSkill = await _coursesSkillsRepository.GetCourseSkillByIdAsync(command.oldCourseSkillId);

                if (oldCourseSkill == null)
                {
                    return Error.Validation(description: "Course and skill do not exist together.");
                }

                if (await _coursesRepository.GetCourseByIdAsync(command.courseId) is not Course course)
                {
                    return Error.Validation(description: "Target course does not exist.");
                }

                if (oldCourseSkill.CourseId != course.Id &&
                    await _coursesSkillsRepository.GetOldCourseAndCourseSkillsCountAsync(oldCourseSkill.CourseId, course.Id) > 3)
                {
                    return Error.Validation(description: "Updating this course skills would make its skills go above 3.");
                }

                if (await _skillsRepository.GetSkillByIdAsync(command.skillId) is not Skill skill)
                {
                    return Error.Validation(description: "Target skill does not exist.");
                }

                if (await _coursesSkillsRepository.CourseSkillExistsByCourseAndSkillIdsAsync(course.Id, skill.Id) == true)
                {
                    return Error.Validation(description: "Updating course skill will cause duplicates.");
                }

                var newCourseSkill = oldCourseSkill.UpdateCourseSkill(
                    courseId: course.Id,
                    skillId: skill.Id,
                    weight: command.weight ?? oldCourseSkill.Weight);

                if (newCourseSkill.IsError)
                {
                    return newCourseSkill.Errors;
                }

                _coursesSkillsRepository.UpdateCoursesSkills(oldCourseSkill, newCourseSkill.Value);

                await _unitOfWork.CommitChangesAsync();

                return new CoursesSkillsDto
                {
                    CourseSkillId = newCourseSkill.Value.Id,
                    CourseId = newCourseSkill.Value.CourseId,
                    Course = course.Subject,
                    SkillId = newCourseSkill.Value.SkillId,
                    Skill = skill.SkillName,
                    Weight = newCourseSkill.Value.Weight,

                    AffectedId = newCourseSkill.Value.Id
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
