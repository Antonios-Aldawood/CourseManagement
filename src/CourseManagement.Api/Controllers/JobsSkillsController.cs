using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using CourseManagement.Contracts.JobsSkills;
using CourseManagement.Application.JobSkill.Commands.CreateJobSkill;
using CourseManagement.Application.JobSkill.Commands.UpdateJobSkill;
using CourseManagement.Application.JobSkill.Commands.UpdateJobSkillJob;
using CourseManagement.Application.JobSkill.Commands.UpdateJobSkillSkill;
using CourseManagement.Application.JobSkill.Queries.GetAllJobsSkills;
using CourseManagement.Application.JobSkill.Queries.GetJobSkillJobs;
using CourseManagement.Application.JobSkill.Queries.GetJobSkillSkills;

namespace CourseManagement.Api.Controllers
{
    [Route("[controller]")]
    public class JobsSkillsController(ISender _mediator) : ApiController
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddJobSkill(CreateJobSkillRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new CreateJobSkillCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                jobTitle: request.JobTitle,
                skillSkillName: request.SkillSkillName,
                weight: request.Weight);

            var createJobSkillResult = await _mediator.Send(command);

            return createJobSkillResult.Match(
                jobSkill => CreatedAtAction(
                    nameof(ListJobsSkills),
                    new JobsSkillsResponse(
                        JobSkillId: jobSkill.JobSkillId,
                        DepartmentId: jobSkill.DepartmentId,
                        Department: jobSkill.DepartmentName,
                        JobId: jobSkill.JobId,
                        Job: jobSkill.JobTitle,
                        SkillId: jobSkill.SkillId,
                        Skill: jobSkill.Skill,
                        Weight: jobSkill.Weight)),
                Problem);
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> EditJobSkill(EditJobSkillRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new UpdateJobSkillCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                oldJobSkillId: request.OldJobSkillId,
                jobId: request.JobId,
                skillId: request.SkillId,
                weight: request.Weight);

            var createJobSkillResult = await _mediator.Send(command);

            return createJobSkillResult.Match(
                jobSkill => CreatedAtAction(
                    nameof(ListJobsSkills),
                    new JobsSkillsResponse(
                        JobSkillId: jobSkill.JobSkillId,
                        JobId: jobSkill.JobId,
                        SkillId: jobSkill.SkillId,
                        Weight: jobSkill.Weight)),
                Problem);
        }

        [Authorize]
        [HttpPut("Job")]
        public async Task<IActionResult> EditJobSkillJob(EditJobsSkillsRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new UpdateJobSkillJobCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                oldJobId: request.OldId,
                jobId: request.Id,
                weight: request.Weight);

            var createJobSkillResult = await _mediator.Send(command);

            return createJobSkillResult.Match(
                jobsSkills => CreatedAtAction(
                    nameof(ListJobsSkills),
                    jobsSkills.ConvertAll(
                        jobSkill => new JobsSkillsResponse(
                            JobSkillId: jobSkill.JobSkillId,
                            JobId: jobSkill.JobId,
                            SkillId: jobSkill.SkillId,
                            Weight: jobSkill.Weight))),
                Problem);
        }

        [Authorize]
        [HttpPut("Skill")]
        public async Task<IActionResult> EditJobSkillSkill(EditJobsSkillsRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new UpdateJobSkillSkillCommand(
                ipAddress: GetClientIp(),
                oldSkillId: request.OldId,
                skillId: request.Id,
                headers: headersDictionary,

                weight: request.Weight);

            var createJobSkillResult = await _mediator.Send(command);

            return createJobSkillResult.Match(
                jobsSkills => CreatedAtAction(
                    nameof(ListJobsSkills),
                    jobsSkills.ConvertAll(
                        jobSkill => new JobsSkillsResponse(
                            JobSkillId: jobSkill.JobSkillId,
                            JobId: jobSkill.JobId,
                            SkillId: jobSkill.SkillId,
                            Weight: jobSkill.Weight))),
                Problem);
        }

        [Authorize]
        [HttpGet("Job")]
        public async Task<IActionResult> GetJobsSkillsForJob(int JobId)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetJobSkillSkillsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                jobId: JobId);

            var getJobSkillsResult = await _mediator.Send(query);

            return getJobSkillsResult.Match(
                jobsSkills => Ok(
                    jobsSkills.ConvertAll(
                        jobSkill => new JobsSkillsResponse(
                            JobSkillId: jobSkill.JobSkillId,
                            DepartmentId: jobSkill.DepartmentId,
                            Department: jobSkill.DepartmentName,
                            JobId: jobSkill.JobId,
                            Job: jobSkill.JobTitle,
                            SkillId: jobSkill.SkillId,
                            Skill: jobSkill.Skill,
                            Weight: jobSkill.Weight))),
                Problem);
        }

        [Authorize]
        [HttpGet("Skill")]
        public async Task<IActionResult> GetJobsSkillsForSkill(int SkillId)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetJobSkillJobsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                skillId: SkillId);

            var getJobSkillsResult = await _mediator.Send(query);

            return getJobSkillsResult.Match(
                jobsSkills => Ok(
                    jobsSkills.ConvertAll(
                        jobSkill => new JobsSkillsResponse(
                            JobSkillId: jobSkill.JobSkillId,
                            DepartmentId: jobSkill.DepartmentId,
                            Department: jobSkill.DepartmentName,
                            JobId: jobSkill.JobId,
                            Job: jobSkill.JobTitle,
                            SkillId: jobSkill.SkillId,
                            Skill: jobSkill.Skill,
                            Weight: jobSkill.Weight))),
                Problem);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListJobsSkills()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetAllJobsSkillsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var getJobSkillsResult = await _mediator.Send(query);

            return getJobSkillsResult.Match(
                jobsSkills => Ok(
                    jobsSkills.ConvertAll(
                        jobSkill => new JobsSkillsResponse(
                            JobSkillId: jobSkill.JobSkillId,
                            DepartmentId: jobSkill.DepartmentId,
                            Department: jobSkill.DepartmentName,
                            JobId: jobSkill.JobId,
                            Job: jobSkill.JobTitle,
                            SkillId: jobSkill.SkillId,
                            Skill: jobSkill.Skill,
                            Weight: jobSkill.Weight))),
                Problem);
        }
    }
}
