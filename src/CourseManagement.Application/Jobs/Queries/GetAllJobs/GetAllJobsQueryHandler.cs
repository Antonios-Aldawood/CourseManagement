using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Jobs.Common.Dto;

namespace CourseManagement.Application.Jobs.Queries.GetAllJobs
{
    public class GetAllJobsQueryHandler(
        IJobsRepository jobsRepository
        ) : IRequestHandler<GetAllJobsQuery, ErrorOr<List<JobDto>>>
    {
        private readonly IJobsRepository _jobsRepository = jobsRepository;

        public async Task<ErrorOr<List<JobDto>>> Handle(GetAllJobsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<JobDto> jobs = await _jobsRepository.GetAllJobsAsync();

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
