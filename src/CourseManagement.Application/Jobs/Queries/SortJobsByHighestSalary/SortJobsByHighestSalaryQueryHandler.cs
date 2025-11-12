using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Jobs.Common.Dto;

namespace CourseManagement.Application.Jobs.Queries.SortJobsByHighestSalary
{
    public class SortJobsByHighestSalaryQueryHandler(
        IJobsRepository jobsRepository
        ) : IRequestHandler<SortJobsByHighestSalaryQuery, ErrorOr<List<JobDto>>>
    {
        private readonly IJobsRepository _jobsRepository = jobsRepository;

        public async Task<ErrorOr<List<JobDto>>> Handle(SortJobsByHighestSalaryQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var sortedJobs = await _jobsRepository.SortJobsByHighestSalaryAsync();

                if (sortedJobs is not List<JobDto> jobs ||
                    sortedJobs.Count == 0)
                {
                    return Error.Validation(description: "Jobs not found.");
                }

                jobs.ForEach(job => job.AffectedId = job.JobId);

                return jobs;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
