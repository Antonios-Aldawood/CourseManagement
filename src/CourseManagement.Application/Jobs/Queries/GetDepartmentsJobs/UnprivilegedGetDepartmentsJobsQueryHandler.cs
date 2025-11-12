using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Domain.Departments;
using CourseManagement.Application.Jobs.Common.Dto;

namespace CourseManagement.Application.Jobs.Queries.GetDepartmentsJobs
{
    public class UnprivilegedGetDepartmentsJobsQueryHandler(
        IJobsRepository jobsRepository,
        IDepartmentsRepository departmentsRepository
        ) : IRequestHandler<UnprivilegedGetDepartmentsJobsQuery, ErrorOr<List<JobDto>>>
    {
        private readonly IJobsRepository _jobsRepository = jobsRepository;
        private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;

        public async Task<ErrorOr<List<JobDto>>> Handle(UnprivilegedGetDepartmentsJobsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<Department> departments = await _departmentsRepository.GetDepartmentsAsync(query.departmentNames);

                if (departments is null ||
                    departments.Count == 0)
                {
                    return Error.Validation(description: "Departments not found.");
                }

                List<int> departmentIds = departments.Select(d => d.Id).ToList();

                List<JobDto> jobs = await _jobsRepository.UnprivilegedGetDepartmentsJobsAsync(departmentIds);

                if (jobs is null ||
                    jobs.Count == 0)
                {
                    return Error.Validation(description: "Departments have no jobs.");
                }

                jobs.ForEach(jR => jR.AffectedId = jR.JobId);

                return jobs;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
