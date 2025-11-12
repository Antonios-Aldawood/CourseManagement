using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.CourseSkill.Common.Dto;
using CourseManagement.Domain.CoursesSkills;

namespace CourseManagement.Application.CourseSkill.Commands.CreateCourseSkill
{
    public class CreateCourseSkillCommandHandler(
        ICoursesSkillsRepository coursesSkillsRepository,
        ICoursesRepository coursesRepository,
        ISkillsRepository skillsRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<CreateCourseSkillCommand, ErrorOr<CoursesSkillsDto>>
    {
        private readonly ICoursesSkillsRepository _coursesSkillsRepository = coursesSkillsRepository;
        private readonly ICoursesRepository _coursesRepository = coursesRepository;
        private readonly ISkillsRepository _skillsRepository = skillsRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<CoursesSkillsDto>> Handle(CreateCourseSkillCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var course = await _coursesRepository.GetCourseByIdAsync(command.courseId);

                if (course == null)
                {
                    return Error.Validation(description: "Course does not exist.");
                }

                var skill = await _skillsRepository.GetSkillByIdAsync(command.skillId);

                if (skill == null)
                {
                    return Error.Validation(description: "Skill does not exist.");
                }

                List<CoursesSkills> coursesSkills = await _coursesSkillsRepository.GetCoursesSkillsByCourseIdAsync(course.Id);

                if (coursesSkills is not null &&
                    coursesSkills.Count >= 3)
                {
                    return Error.Validation(description: "Course can not teach more than 3 skills.");
                }

                if (coursesSkills is not null &&
                    coursesSkills.Where(cs => cs.CourseId == course.Id && cs.SkillId == skill.Id).FirstOrDefault() is not null)
                {
                    return Error.Validation(description: "Course already has skill.");
                }

                var courseSkill = CoursesSkills.CreateCourseSkill(
                    courseId: command.courseId,
                    skillId: command.skillId,
                    weight: command.weight);

                if (courseSkill.IsError)
                {
                    return courseSkill.Errors;
                }

                await _coursesSkillsRepository.AddCoursesSkillsAsync(courseSkill.Value);

                await _unitOfWork.CommitChangesAsync();

                return new CoursesSkillsDto
                {
                    CourseSkillId = courseSkill.Value.Id,
                    CourseId = courseSkill.Value.CourseId,
                    Course = course.Subject,
                    SkillId = courseSkill.Value.SkillId,
                    Skill = skill.SkillName,
                    Weight = courseSkill.Value.Weight,

                    AffectedId = courseSkill.Value.Id
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
