using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.CourseSkill.Common.Dto;

namespace CourseManagement.Application.CourseSkill.Queries.GetCourseSkillSkills
{
    public class GetCourseSkillSkillsQueryHandler(
        ICoursesSkillsRepository coursesSkillsRepository
        ) : IRequestHandler<GetCourseSkillSkillsQuery, ErrorOr<List<CoursesSkillsDto>>>
    {
        private readonly ICoursesSkillsRepository _coursesSkillsRepository = coursesSkillsRepository;

        public async Task<ErrorOr<List<CoursesSkillsDto>>> Handle(GetCourseSkillSkillsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<CoursesSkillsDto> foundCourseSkills = await _coursesSkillsRepository.GetCoursesSkillsWithNamesByCourseIdAsync(query.courseId);

                if (foundCourseSkills is null ||
                    foundCourseSkills.Count == 0)
                {
                    return Error.Validation(description: "No skills for course found.");
                }

                foundCourseSkills.ForEach(cs => cs.AffectedId = cs.CourseSkillId);

                return foundCourseSkills;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
