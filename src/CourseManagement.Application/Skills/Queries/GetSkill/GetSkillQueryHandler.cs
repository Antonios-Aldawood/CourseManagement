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

namespace CourseManagement.Application.Skills.Queries.GetSkill
{
    public class GetSkillQueryHandler(
        ISkillsRepository skillsRepository
        ) : IRequestHandler<GetSkillQuery, ErrorOr<List<SkillSoleDto>>>
    {
        private readonly ISkillsRepository _skillsRepository = skillsRepository;

        public async Task<ErrorOr<List<SkillSoleDto>>> Handle(GetSkillQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var foundSkills = await _skillsRepository.GetSkillBySkillNameAsync(query.skillName);

                if (foundSkills is not List<Skill> skills ||
                    foundSkills.Count == 0)
                {
                    return Error.Validation(description: "Skill not found.");
                }

                List<SkillSoleDto> skillResponse = [];

                foreach (Skill skill in skills)
                {
                    SkillSoleDto dto = SkillSoleDto.AddDto(skill);

                    skillResponse.Add(dto);
                }

                return skillResponse;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
