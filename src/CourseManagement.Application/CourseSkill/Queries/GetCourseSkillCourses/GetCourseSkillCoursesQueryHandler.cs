using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.CourseSkill.Common.Dto;

namespace CourseManagement.Application.CourseSkill.Queries.GetCourseSkillCourses
{
    public class GetCourseSkillCoursesQueryHandler(
        ICoursesSkillsRepository coursesSkillsRepository
        ) : IRequestHandler<GetCourseSkillCoursesQuery, ErrorOr<List<CoursesSkillsDto>>>
    {
        private readonly ICoursesSkillsRepository _coursesSkillsRepository = coursesSkillsRepository;

        public async Task<ErrorOr<List<CoursesSkillsDto>>> Handle(GetCourseSkillCoursesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<CoursesSkillsDto> foundCoursesSkills = await _coursesSkillsRepository.GetCoursesSkillsWithSubjectsBySkillIdAsync(query.skillId);

                if (foundCoursesSkills is null ||
                    foundCoursesSkills.Count == 0)
                {
                    return Error.Validation(description: "Skill given to no courses.");
                }

                foundCoursesSkills.ForEach(cs => cs.AffectedId = cs.CourseSkillId);

                return foundCoursesSkills;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
