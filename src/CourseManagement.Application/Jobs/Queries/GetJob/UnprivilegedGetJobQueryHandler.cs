using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Jobs.Common.Dto;

namespace CourseManagement.Application.Jobs.Queries.GetJob
{
    public class UnprivilegedGetJobQueryHandler(
        IJobsRepository jobsRepository
        ) : IRequestHandler<UnprivilegedGetJobQuery, ErrorOr<List<JobDto>>>
    {
        private readonly IJobsRepository _jobsRepository = jobsRepository;

        public async Task<ErrorOr<List<JobDto>>> Handle(UnprivilegedGetJobQuery query, CancellationToken cancellationToken)
        {
            try
            {
                //if (query.title == "THROW")
                //{
                //    throw new DivideByZeroException();
                //}

                var foundJobs = await _jobsRepository.UnprivilegedGetJobByNameAsync(query.title);

                if (foundJobs is not List<JobDto> jobs ||
                    foundJobs.Count == 0)
                {
                    return Error.Validation(description: "Job not found.");
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
