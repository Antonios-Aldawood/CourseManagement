using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using CourseManagement.Contracts.Skills;
using CourseManagement.Application.Skills.Commands.CreateSkill;
using CourseManagement.Application.Skills.Commands.UpdateSkill;
using CourseManagement.Application.Skills.Queries.GetSkill;
using CourseManagement.Application.Skills.Queries.GetAllSkills;
using CourseManagement.Application.Skills.Queries.FilterSkillsByLevelCap;
using CourseManagement.Application.Skills.Queries.SortSkillsByLeastLevelCap;
using CourseManagement.Application.Skills.Queries.SortSkillsByMostLevelCap;

namespace CourseManagement.Api.Controllers
{
    [Route("[controller]")]
    public class SkillsController(ISender _mediator) : ApiController
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddSkill(CreateSkillRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new CreateSkillCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                skillName: request.SkillName,
                levelCap: request.LevelCap,
                courseId: request.CourseId,
                weight: request.Weight);

            var addSkillResult = await _mediator.Send(command);

            return addSkillResult.Match(
                skill => CreatedAtAction(
                    nameof(GetSkill),
                    new SkillResponse(
                        SkillId: skill.SkillId,
                        SkillName: skill.SkillName,
                        LevelCap: skill.LevelCap,
                        Course: skill.Course,
                        CourseSkillId: skill.CourseSkillId,
                        Weight: skill.Weight)),
                Problem);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> EditSkill(EditSkillRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new UpdateSkillCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                oldSkillSkillName: request.OldSkillSkillName,
                skillName: request.SkillName,
                levelCap: request.LevelCap);

            var addSkillResult = await _mediator.Send(command);

            return addSkillResult.Match(
                skill => CreatedAtAction(
                    nameof(GetSkill),
                    new SkillResponse(
                        SkillId: skill.SkillId,
                        SkillName: skill.SkillName,
                        LevelCap: skill.LevelCap)),
                Problem);
        }

        [Authorize]
        [HttpGet("Skill")]
        public async Task<IActionResult> GetSkill(string SkillName)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetSkillQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                skillName: SkillName);

            var getSkillResult = await _mediator.Send(query);

            return getSkillResult.Match(
                skills => Ok(
                    skills.ConvertAll(
                        skill => new SkillResponse(
                            SkillId: skill.SkillId,
                            SkillName: skill.SkillName,
                            LevelCap: skill.LevelCap))),
                Problem);
        }

        [Authorize]
        [HttpGet("LevelCapFilter")]
        public async Task<IActionResult> FilterSkillsLevelCap(int LevelCap)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new FilterSkillsByLevelCapQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                levelCap: LevelCap);

            var filterResult = await _mediator.Send(query);

            return filterResult.Match(
                skills => Ok(
                    skills.ConvertAll(
                        skill => new SkillResponse(
                            SkillId: skill.SkillId,
                            SkillName: skill.SkillName,
                            LevelCap: skill.LevelCap))),
                Problem);
        }

        [Authorize]
        [HttpGet("SortLeastLevel")]
        public async Task<IActionResult> SortSkillsLeastLevel()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new SortSkillsByLeastLevelCapQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var filterResult = await _mediator.Send(query);

            return filterResult.Match(
                skills => Ok(
                    skills.ConvertAll(
                        skill => new SkillResponse(
                            SkillId: skill.SkillId,
                            SkillName: skill.SkillName,
                            LevelCap: skill.LevelCap))),
                Problem);
        }

        [Authorize]
        [HttpGet("SortMostLevel")]
        public async Task<IActionResult> SortSkillsMostLevel()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new SortSkillsByMostLevelCapQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var filterResult = await _mediator.Send(query);

            return filterResult.Match(
                skills => Ok(
                    skills.ConvertAll(
                        skill => new SkillResponse(
                            SkillId: skill.SkillId,
                            SkillName: skill.SkillName,
                            LevelCap: skill.LevelCap))),
                Problem);
        }

        [Authorize]
        [HttpGet("Paginated")]
        public async Task<IActionResult> ListSkills([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new PaginatedGetAllSkillsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                pageNumber: pageNumber,
                pageSize: pageSize);

            var listSkillsResult = await _mediator.Send(query);

            return listSkillsResult.Match(
                paged => Ok(new
                {
                    paged.PageNumber,
                    paged.PageSize,
                    paged.TotalCount,
                    paged.TotalPages,
                    Items = paged.Items.Select(s => new SkillResponse(
                        SkillId: s.SkillId,
                        SkillName: s.SkillName,
                        LevelCap: s.LevelCap
                    ))
                }),
                Problem);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListSkills()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetAllSkillsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var listSkillsResult = await _mediator.Send(query);

            return listSkillsResult.Match(
                skills => Ok(
                    skills.ConvertAll(
                        skill => new SkillResponse(
                            SkillId: skill.SkillId,
                            SkillName: skill.SkillName,
                            LevelCap: skill.LevelCap))),
                Problem);
        }
    }
}
