using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.JobSkill.Common.Dto;

namespace CourseManagement.Application.JobSkill.Queries.GetJobSkillSkills
{
    public class GetJobSkillSkillsQueryHandler(
        IJobsSkillsRepository jobsSkillsRepository
        ) : IRequestHandler<GetJobSkillSkillsQuery, ErrorOr<List<JobsSkillsDto>>>
    {
        private readonly IJobsSkillsRepository _jobsSkillsRepository = jobsSkillsRepository;

        public async Task<ErrorOr<List<JobsSkillsDto>>> Handle(GetJobSkillSkillsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<JobsSkillsDto> foundJobsSkills = await _jobsSkillsRepository.GetJobsSkillsWithNamesByJobIdAsync(query.jobId);

                if (foundJobsSkills is null ||
                    foundJobsSkills.Count == 0)
                {
                    return Error.Validation(description: "Job has no skills.");
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
