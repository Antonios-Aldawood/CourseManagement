using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Skills.Common.Dto;
using CourseManagement.Domain.Skills;

namespace CourseManagement.Application.Skills.Queries.SortSkillsByMostLevelCap
{
    public class SortSkillsByMostLevelCapQueryHandler(
        ISkillsRepository skillsRepository
        ) : IRequestHandler<SortSkillsByMostLevelCapQuery, ErrorOr<List<SkillSoleDto>>>
    {
        private readonly ISkillsRepository _skillsRepository = skillsRepository;

        public async Task<ErrorOr<List<SkillSoleDto>>> Handle(SortSkillsByMostLevelCapQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var foundSkills = await _skillsRepository.SortSkillsByMostLevelCapAsync();

                if (foundSkills is not List<Skill> skills ||
                    foundSkills.Count == 0)
                {
                    return Error.Validation(description: "Skills not found.");
                }

                List<SkillSoleDto> skillsResponse = [];

                foreach (Skill skill in skills)
                {
                    SkillSoleDto dto = SkillSoleDto.AddDto(skill);

                    skillsResponse.Add(dto);
                }

                return skillsResponse;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
