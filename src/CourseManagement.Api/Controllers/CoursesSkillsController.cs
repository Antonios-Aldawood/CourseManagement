using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using CourseManagement.Contracts.CoursesSkills;
using CourseManagement.Application.CourseSkill.Commands.CreateCourseSkill;
using CourseManagement.Application.CourseSkill.Commands.UpdateCourseSkill;
using CourseManagement.Application.CourseSkill.Commands.UpdateCourseSkillCourse;
using CourseManagement.Application.CourseSkill.Commands.UpdateCourseSkillSkill;
using CourseManagement.Application.CourseSkill.Queries.GetAllCoursesSkills;
using CourseManagement.Application.CourseSkill.Queries.GetCourseSkillCourses;
using CourseManagement.Application.CourseSkill.Queries.GetCourseSkillSkills;

namespace CourseManagement.Api.Controllers
{
    [Route("[controller]")]
    public class CoursesSkillsController(ISender _mediator) : ApiController
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCourseSkill(CreateCourseSkillRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new CreateCourseSkillCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                courseId: request.CourseId,
                skillId: request.SkillId,
                weight: request.Weight);

            var createCourseSkillResult = await _mediator.Send(command);

            return createCourseSkillResult.Match(
                courseSkill => CreatedAtAction(
                    nameof(ListCoursesSkills),
                    new CoursesSkillsResponse(
                        CourseSkillId: courseSkill.CourseSkillId,
                        CourseId: courseSkill.CourseId,
                        Course: courseSkill.Course,
                        SkillId: courseSkill.SkillId,
                        Skill: courseSkill.Skill,
                        Weight: courseSkill.Weight)),
                Problem);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> EditCourseSkill(EditCourseSkillRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new UpdateCourseSkillCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                oldCourseSkillId: request.OldCourseSkillId,
                courseId: request.CourseId,
                skillId: request.SkillId,
                weight: request.Weight);

            var createCourseSkillResult = await _mediator.Send(command);

            return createCourseSkillResult.Match(
                courseSkill => CreatedAtAction(
                    nameof(ListCoursesSkills),
                    new CoursesSkillsResponse(
                        CourseSkillId: courseSkill.CourseSkillId,
                        CourseId: courseSkill.CourseId,
                        Course: courseSkill.Course,
                        SkillId: courseSkill.SkillId,
                        Skill: courseSkill.Skill,
                        Weight: courseSkill.Weight)),
                Problem);
        }

        [Authorize]
        [HttpPut("Course")]
        public async Task<IActionResult> EditCourseSkillCourse(EditCoursesSkillsRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new UpdateCourseSkillCourseCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                oldCourseId: request.OldId,
                courseId: request.Id,
                weight: request.Weight);

            var createCourseSkillResult = await _mediator.Send(command);

            return createCourseSkillResult.Match(
                coursesSkills => CreatedAtAction(
                    nameof(ListCoursesSkills),
                    coursesSkills.ConvertAll(
                        courseSkill => new CoursesSkillsResponse(
                            CourseSkillId: courseSkill.CourseSkillId,
                            CourseId: courseSkill.CourseId,
                            SkillId: courseSkill.SkillId,
                            Weight: courseSkill.Weight))),
                Problem);
        }

        [Authorize]
        [HttpPut("Skill")]
        public async Task<IActionResult> EditCourseSkillSkill(EditCoursesSkillsRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new UpdateCourseSkillSkillCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                oldSkillId: request.OldId,
                skillId: request.Id,
                weight: request.Weight);

            var createCourseSkillResult = await _mediator.Send(command);

            return createCourseSkillResult.Match(
                coursesSkills => CreatedAtAction(
                    nameof(ListCoursesSkills),
                    coursesSkills.ConvertAll(
                        courseSkill => new CoursesSkillsResponse(
                            CourseSkillId: courseSkill.CourseSkillId,
                            CourseId: courseSkill.CourseId,
                            SkillId: courseSkill.SkillId,
                            Weight: courseSkill.Weight))),
                Problem);
        }

        [Authorize]
        [HttpGet("Course")]
        public async Task<IActionResult> GetCoursesSkillsForCourse(int CourseId)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new GetCourseSkillSkillsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                courseId: CourseId);

            var getCourseSkillsResult = await _mediator.Send(query);

            return getCourseSkillsResult.Match(
                coursesSkills => Ok(
                    coursesSkills.ConvertAll(
                        courseSkill => new CoursesSkillsResponse(
                            CourseSkillId: courseSkill.CourseSkillId,
                            CourseId: courseSkill.CourseId,
                            Course: courseSkill.Course,
                            SkillId: courseSkill.SkillId,
                            Skill: courseSkill.Skill,
                            Weight: courseSkill.Weight))),
                Problem);
        }

        [Authorize]
        [HttpGet("Skill")]
        public async Task<IActionResult> GetCoursesSkillsForSkill(int SkillId)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetCourseSkillCoursesQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                skillId: SkillId);

            var getCourseSkillsResult = await _mediator.Send(query);

            return getCourseSkillsResult.Match(
                coursesSkills => Ok(
                    coursesSkills.ConvertAll(
                        courseSkill => new CoursesSkillsResponse(
                            CourseSkillId: courseSkill.CourseSkillId,
                            CourseId: courseSkill.CourseId,
                            Course: courseSkill.Course,
                            SkillId: courseSkill.SkillId,
                            Skill: courseSkill.Skill,
                            Weight: courseSkill.Weight))),
                Problem);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListCoursesSkills()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetAllCoursesSkillsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var listCoursesSkillsResult = await _mediator.Send(query);

            return listCoursesSkillsResult.Match(
                coursesSkills => Ok(
                    coursesSkills.ConvertAll(
                        courseSkill => new CoursesSkillsResponse(
                            CourseSkillId: courseSkill.CourseSkillId,
                            CourseId: courseSkill.CourseId,
                            Course: courseSkill.Course,
                            SkillId: courseSkill.SkillId,
                            Skill: courseSkill.Skill,
                            Weight: courseSkill.Weight))),
                Problem);
        }
    }
}
