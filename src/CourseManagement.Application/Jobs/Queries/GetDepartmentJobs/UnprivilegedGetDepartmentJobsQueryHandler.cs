using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Jobs.Common.Dto;

namespace CourseManagement.Application.Jobs.Queries.GetDepartmentJobs
{
    public class UnprivilegedGetDepartmentJobsQueryHandler(
        IJobsRepository jobsRepository,
        IDepartmentsRepository departmentsRepository
        ) : IRequestHandler<UnprivilegedGetDepartmentJobsQuery, ErrorOr<List<JobDto>>>
    {
        private readonly IJobsRepository _jobsRepository = jobsRepository;
        private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;

        public async Task<ErrorOr<List<JobDto>>> Handle(UnprivilegedGetDepartmentJobsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var department = await _departmentsRepository.GetByExactNameAsync(query.departmentName);
                if (department == null)
                {
                    return Error.Validation(description: "Department not found.");
                }

                List<JobDto> jobs = await _jobsRepository.UnprivilegedGetDepartmentJobsAsync(department.Id);
                if (jobs is null ||
                    jobs.Count == 0)
                {
                    return Error.Validation(description: "Department has no jobs.");
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
