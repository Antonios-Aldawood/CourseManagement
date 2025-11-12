using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.JobSkill.Common.Dto;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.JobSkill.Queries.GetAllJobsSkills
{
    public class GetAllJobsSkillsQueryHandler(
        IJobsSkillsRepository jobsSkillsRepository
        ) : IRequestHandler<GetAllJobsSkillsQuery, ErrorOr<List<JobsSkillsDto>>>
    {
        private readonly IJobsSkillsRepository _jobsSkillsRepository = jobsSkillsRepository;

        public async Task<ErrorOr<List<JobsSkillsDto>>> Handle(GetAllJobsSkillsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<JobsSkillsDto> jobsSkills = await _jobsSkillsRepository.GetAllJobsSkillsWithTitlesAndNamesAsync();

                jobsSkills.ForEach(js => js.AffectedId = js.JobSkillId);

                return jobsSkills;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
