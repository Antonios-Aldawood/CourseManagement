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

namespace CourseManagement.Application.Skills.Queries.GetAllSkills
{
    public class GetAllSkillsQueryHandler(
        ISkillsRepository skillsRepository
        ) : IRequestHandler<GetAllSkillsQuery, ErrorOr<List<SkillSoleDto>>>
    {
        private readonly ISkillsRepository _skillsRepository = skillsRepository;

        public async Task<ErrorOr<List<SkillSoleDto>>> Handle(GetAllSkillsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<Skill> skills = await _skillsRepository.GetAllSkillsAsync();
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
