using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.CourseSkill.Common.Dto;

namespace CourseManagement.Application.CourseSkill.Queries.GetAllCoursesSkills
{
    public class GetAllCoursesSkillsQueryHandler(
        ICoursesSkillsRepository coursesSkillsRepository
        ) : IRequestHandler<GetAllCoursesSkillsQuery, ErrorOr<List<CoursesSkillsDto>>>
    {
        private readonly ICoursesSkillsRepository _coursesSkillsRepository = coursesSkillsRepository;

        public async Task<ErrorOr<List<CoursesSkillsDto>>> Handle(GetAllCoursesSkillsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<CoursesSkillsDto> coursesSkills = await _coursesSkillsRepository.GetAllCoursesSkillsWithSubjectsAndNamesAsync();

                coursesSkills.ForEach(cs => cs.AffectedId = cs.CourseSkillId);

                return coursesSkills;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
