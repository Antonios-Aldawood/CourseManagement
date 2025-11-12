using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Jobs.Common.Dto;

namespace CourseManagement.Application.Jobs.Queries.GetJobsByDepartment
{
    public class UnprivilegedGetJobsByDepartmentQueryHandler(
        IJobsRepository jobsRepository
        ) : IRequestHandler<UnprivilegedGetJobsByDepartmentQuery, ErrorOr<List<JobDto>>>
    {
        private readonly IJobsRepository _jobsRepository = jobsRepository;

        public async Task<ErrorOr<List<JobDto>>> Handle(UnprivilegedGetJobsByDepartmentQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var foundJobs = await _jobsRepository.GetUnprivilegedJobsByDepartmentAsync(query.name);

                if (foundJobs is not List<JobDto> jobs ||
                    foundJobs.Count == 0)
                {
                    return Error.Validation(description: "Jobs for this department not found.");
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
