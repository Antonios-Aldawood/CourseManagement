using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.JobSkill.Common.Dto;

namespace CourseManagement.Application.JobSkill.Queries.GetJobSkillJobs
{
    public class GetJobSkillJobsQueryHandler(
        IJobsSkillsRepository jobsSkillsRepository
        ) : IRequestHandler<GetJobSkillJobsQuery, ErrorOr<List<JobsSkillsDto>>>
    {
        private readonly IJobsSkillsRepository _jobsSkillsRepository = jobsSkillsRepository;

        public async Task<ErrorOr<List<JobsSkillsDto>>> Handle(GetJobSkillJobsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<JobsSkillsDto> foundJobsSkills = await _jobsSkillsRepository.GetJobsSkillsWithTitlesBySkillIdAsync(query.skillId);

                if (foundJobsSkills is null ||
                    foundJobsSkills.Count == 0)
                {
                    return Error.Validation(description: "Skill given to no jobs.");
                }

                foundJobsSkills.ForEach(js => js.AffectedId = js.JobSkillId);

                return foundJobsSkills;
            }
            catch (Exception ex) 
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
