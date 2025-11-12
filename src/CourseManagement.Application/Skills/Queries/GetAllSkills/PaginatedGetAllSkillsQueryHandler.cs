using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Common.Dto;
using CourseManagement.Application.Skills.Common.Dto;

namespace CourseManagement.Application.Skills.Queries.GetAllSkills
{
    public class PaginatedGetAllSkillsQueryHandler(
        ISkillsRepository skillsRepository
        ) : IRequestHandler<PaginatedGetAllSkillsQuery, ErrorOr<PagedResult<SkillSoleDto>>>
    {
        private readonly ISkillsRepository _skillsRepository = skillsRepository;

        public async Task<ErrorOr<PagedResult<SkillSoleDto>>> Handle(PaginatedGetAllSkillsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var (skills, totalCount) = await _skillsRepository
                    .GetAllSkillsPaginatedAsync(query.pageNumber, query.pageSize);

                var skillResponse = skills
                    .Select(SkillSoleDto.AddDto)
                    .ToList();

                return new PagedResult<SkillSoleDto>
                {
                    Items = skillResponse,
                    PageNumber = query.pageNumber,
                    PageSize = query.pageSize,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
