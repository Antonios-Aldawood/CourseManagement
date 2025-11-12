using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Common.Dto;
using CourseManagement.Application.Jobs.Common.Dto;

namespace CourseManagement.Application.Jobs.Queries.GetAllJobs
{
    public class UnprivilegedPaginatedGetAllJobsQueryHandler(
        IJobsRepository jobsRepository
        ) : IRequestHandler<UnprivilegedPaginatedGetAllJobsQuery, ErrorOr<PagedResult<JobDto>>>
    {
        private readonly IJobsRepository _jobsRepository = jobsRepository;

        public async Task<ErrorOr<PagedResult<JobDto>>> Handle(UnprivilegedPaginatedGetAllJobsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var (jobs, totalCount) = await _jobsRepository
                    .UnprivilegedGetAllPaginatedJobsAsync(query.pageNumber, query.pageSize);

                jobs.ForEach(job => job.AffectedId = job.JobId);

                return new PagedResult<JobDto>
                {
                    Items = jobs,
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
