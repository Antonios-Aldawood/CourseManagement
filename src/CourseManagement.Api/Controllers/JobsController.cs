using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using CourseManagement.Contracts.Jobs;
using CourseManagement.Application.Jobs.Commands.CreateJob;
using CourseManagement.Application.Jobs.Commands.UpdateJob;
using CourseManagement.Application.Jobs.Queries.FilterJobsBySalary;
using CourseManagement.Application.Jobs.Queries.GetAllJobs;
using CourseManagement.Application.Jobs.Queries.GetDepartmentJobs;
using CourseManagement.Application.Jobs.Queries.GetDepartmentsJobs;
using CourseManagement.Application.Jobs.Queries.GetJob;
using CourseManagement.Application.Jobs.Queries.GetJobsByDepartment;
using CourseManagement.Application.Jobs.Queries.SortJobsByHighestSalary;
using CourseManagement.Application.Jobs.Queries.SortJobsByLowestSalary;

namespace CourseManagement.Api.Controllers
{
    [Route("[controller]")]
    public class JobsController(ISender _mediator) : ApiController
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddJob(AddJobRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new CreateJobCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                title: request.Title,
                minSalary: request.MinSalary,
                maxSalary: request.MaxSalary,
                description: request.Description,
                departmentName: request.DepartmentName);

            var addDepartmentJobResult = await _mediator.Send(command);

            return addDepartmentJobResult.Match(
                job => CreatedAtAction(
                    nameof(GetJob),
                    new JobResponse(
                        JobId: job.JobId,
                        Title: job.Title,
                        MinSalary: job.MinSalary,
                        MaxSalary: job.MaxSalary,
                        Description: job.Description,
                        Department: job.Department)),
                Problem);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> EditJob(EditJobRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new UpdateJobCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                oldJobTitle: request.OldTitle,
                title: request.Title,
                minSalary: request.MinSalary,
                maxSalary: request.MaxSalary,
                description: request.Description,
                departmentId: request.DepartmentId);

            var updateDepartmentJobResult = await _mediator.Send(command);

            return updateDepartmentJobResult.Match(
                job => CreatedAtAction(
                    nameof(GetJob),
                    new JobResponse(
                        JobId: job.JobId,
                        Title: job.Title,
                        MinSalary: job.MinSalary,
                        MaxSalary: job.MaxSalary,
                        Description: job.Description,
                        Department: job.Department)),
                Problem);
        }

        [Authorize]
        [HttpPost("Departments")]
        public async Task<IActionResult> GetDepartmentsJobs([FromBody] GetDepartmentsJobsRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetDepartmentsJobsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                departmentNames: request.Names);

            var getDepartmentsJobsResult = await _mediator.Send(query);

            return getDepartmentsJobsResult.Match(
                jobs => Ok(
                    jobs.ConvertAll(
                        job => new JobResponse(
                            JobId: job.JobId,
                            Title: job.Title,
                            MinSalary: job.MinSalary,
                            MaxSalary: job.MaxSalary,
                            Description: job.Description,
                            Department: job.Department))),
                Problem);
        }

        [Authorize]
        [HttpPost("UnprivilegedJobs/Departments")]
        public async Task<IActionResult> GetDepartmentsUnprivilegedJobs([FromBody] GetDepartmentsJobsRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new UnprivilegedGetDepartmentsJobsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                departmentNames: request.Names);

            var getDepartmentsJobsResult = await _mediator.Send(query);

            return getDepartmentsJobsResult.Match(
                jobs => Ok(
                    jobs.ConvertAll(
                        job => new JobResponse(
                            JobId: job.JobId,
                            Title: job.Title,
                            MinSalary: job.MinSalary,
                            MaxSalary: job.MinSalary,
                            Description: job.Description,
                            Department: job.Department))),
                Problem);
        }

        [Authorize]
        [HttpGet("Job")]
        public async Task<IActionResult> GetJob(string Title)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetJobQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                title: Title);

            var getJobResult = await _mediator.Send(query);

            return getJobResult.Match(
                jobs => Ok(
                    jobs.ConvertAll(
                        job => new JobResponse(
                            JobId: job.JobId,
                            Title: job.Title,
                            MinSalary: job.MinSalary,
                            MaxSalary: job.MaxSalary,
                            Description: job.Description,
                            Department: job.Department))),
                Problem);
        }

        [Authorize]
        [HttpGet("UnprivilegedJob")]
        public async Task<IActionResult> GetUnprivilegedJob(string Title)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new UnprivilegedGetJobQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                title: Title);

            var getJobResult = await _mediator.Send(query);

            return getJobResult.Match(
                jobs => Ok(
                    jobs.ConvertAll(
                        job => new JobResponse(
                            JobId: job.JobId,
                            Title: job.Title,
                            Description: job.Description,
                            Department: job.Department))),
                Problem);
        }

        [Authorize]
        [HttpGet("SalaryFilter")]
        public async Task<IActionResult> FilterJobsSalary(double Salary)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new FilterJobsBySalaryQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                salary: Salary);

            var filterResult = await _mediator.Send(query);

            return filterResult.Match(
                jobs => Ok(
                    jobs.ConvertAll(
                        job => new JobResponse(
                            JobId: job.JobId,
                            Title: job.Title,
                            MinSalary: job.MinSalary,
                            MaxSalary: job.MaxSalary,
                            Description: job.Description,
                            Department: job.Department))),
                Problem);
        }

        [Authorize]
        [HttpGet("SortHighestSalary")]
        public async Task<IActionResult> SortJobsHighestSalary()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new SortJobsByHighestSalaryQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var sortResult = await _mediator.Send(query);

            return sortResult.Match(
                jobs => Ok(
                    jobs.ConvertAll(
                        job => new JobResponse(
                            JobId: job.JobId,
                            Title: job.Title,
                            MinSalary: job.MinSalary,
                            MaxSalary: job.MaxSalary,
                            Description: job.Description,
                            Department: job.Department))),
                Problem);
        }

        [Authorize]
        [HttpGet("SortLowestSalary")]
        public async Task<IActionResult> SortJobsLowestSalary()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new SortJobsByLowestSalaryQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var sortResult = await _mediator.Send(query);

            return sortResult.Match(
                jobs => Ok(
                    jobs.ConvertAll(
                        job => new JobResponse(
                            JobId: job.JobId,
                            Title: job.Title,
                            MinSalary: job.MinSalary,
                            MaxSalary: job.MaxSalary,
                            Description: job.Description,
                            Department: job.Department))),
                Problem);
        }

        [Authorize]
        [HttpGet("InDepartment")]
        public async Task<IActionResult> GetJobsInDepartment(string Name)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetJobsByDepartmentQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                name: Name);

            var getJobByDepartmentResult = await _mediator.Send(query);

            return getJobByDepartmentResult.Match(
                jobs => Ok(
                    jobs.ConvertAll(
                        job => new JobResponse(
                            JobId: job.JobId,
                            Title: job.Title,
                            MinSalary: job.MinSalary,
                            MaxSalary: job.MaxSalary,
                            Description: job.Description,
                            Department: job.Department))),
                Problem);
        }

        [Authorize]
        [HttpGet("UnprivilegedJobs/InDepartment")]
        public async Task<IActionResult> GetUnprivilegedJobsInDepartment(string Name)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new UnprivilegedGetJobsByDepartmentQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                name: Name);

            var getJobByDepartmentResult = await _mediator.Send(query);

            return getJobByDepartmentResult.Match(
                jobs => Ok(
                    jobs.ConvertAll(
                        job => new JobResponse(
                            JobId: job.JobId,
                            Title: job.Title,
                            Description: job.Description,
                            Department: job.Department))),
                Problem);
        }

        [Authorize]
        [HttpGet("Department")]
        public async Task<IActionResult> GetJobsForDepartment(string Name)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetDepartmentJobsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                Name);

            var getJobsByDepartmentResult = await _mediator.Send(query);

            return getJobsByDepartmentResult.Match(
                jobs => Ok(
                    jobs.ConvertAll(
                        job => new JobResponse(
                            JobId: job.JobId,
                            Title: job.Title,
                            MinSalary: job.MinSalary,
                            MaxSalary: job.MaxSalary,
                            Description: job.Description,
                            Department: job.Department))),
                Problem);
        }

        [Authorize]
        [HttpGet("UnprivilegedJobs/Department")]
        public async Task<IActionResult> GetUnprivilegedJobsForDepartment(string Name)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new UnprivilegedGetDepartmentJobsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                Name);

            var getJobsByDepartmentResult = await _mediator.Send(query);

            return getJobsByDepartmentResult.Match(
                jobs => Ok(
                    jobs.ConvertAll(
                        job => new JobResponse(
                            JobId: job.JobId,
                            Title: job.Title,
                            Description: job.Description,
                            Department: job.Department))),
                Problem);
        }

        [Authorize]
        [HttpGet("Paginated")]
        public async Task<IActionResult> ListPaginatedJobs([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new PaginatedGetAllJobsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                pageNumber: pageNumber,
                pageSize: pageSize);

            var listJobsResult = await _mediator.Send(query);

            return listJobsResult.Match(
                paged => Ok(new
                {
                    paged.PageNumber,
                    paged.PageSize,
                    paged.TotalCount,
                    paged.TotalPages,
                    Items = paged.Items.Select(j => new JobResponse(
                        JobId: j.JobId,
                        Title: j.Title,
                        MinSalary: j.MinSalary,
                        MaxSalary: j.MaxSalary,
                        Description: j.Description,
                        Department: j.Department
                        ))
                }),
                Problem);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListJobs()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetAllJobsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var listJobsResult = await _mediator.Send(query);

            return listJobsResult.Match(
                jobs => Ok(
                    jobs.ConvertAll(
                        job => new JobResponse(
                            JobId: job.JobId,
                            Title: job.Title,
                            MinSalary: job.MinSalary,
                            MaxSalary: job.MaxSalary,
                            Description: job.Description,
                            Department: job.Department))),
                Problem);
        }

        [Authorize]
        [HttpGet("UnprivilegedJobs/Paginated")]
        public async Task<IActionResult> ListPaginatedUnprivilegedJobs([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new UnprivilegedPaginatedGetAllJobsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                pageNumber: pageNumber,
                pageSize: pageSize);

            var listJobsResult = await _mediator.Send(query);

            return listJobsResult.Match(
                paged => Ok(new
                {
                    paged.PageNumber,
                    paged.PageSize,
                    paged.TotalCount,
                    paged.TotalPages,
                    Items = paged.Items.Select(j => new JobResponse(
                        JobId: j.JobId,
                        Title: j.Title,
                        Description: j.Description,
                        Department: j.Department
                        ))
                }),
                Problem);
        }

        [Authorize]
        [HttpGet("UnprivilegedJobs")]
        public async Task<IActionResult> ListUnprivilegedJobs()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new UnprivilegedGetAllJobsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var listJobsResult = await _mediator.Send(query);

            return listJobsResult.Match(
                jobs => Ok(
                    jobs.ConvertAll(
                        job => new JobResponse(
                            JobId: job.JobId,
                            Title: job.Title,
                            Description: job.Description,
                            Department: job.Department))),
                Problem);
        }
    }
}
