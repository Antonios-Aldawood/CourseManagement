using CourseManagement.Application.Courses.Commands.AddCourseEligibility;
using CourseManagement.Application.Courses.Commands.AddCourseSession;
using CourseManagement.Application.Courses.Commands.AddCourseSessionMaterial;
using CourseManagement.Application.Courses.Commands.CreateCourse;
using CourseManagement.Application.Courses.Commands.UpdateCourse;
using CourseManagement.Application.Courses.Commands.UpdateCourseSessionMaterial;
using CourseManagement.Application.Courses.Commands.UpdateCourseSessionMaterialPlacement;
using CourseManagement.Application.Courses.Queries.DownloadCourseSessionMaterial;
using CourseManagement.Application.Courses.Queries.GetAllCourses;
using CourseManagement.Application.Courses.Queries.GetCourse;
using CourseManagement.Application.Courses.Queries.GetCourseEligibilities;
using CourseManagement.Application.Courses.Queries.GetCourseSessionMaterials;
using CourseManagement.Application.Courses.Queries.GetCourseSessions;
using CourseManagement.Application.Courses.Queries.GetCoursesForEligibilities;
using CourseManagement.Application.Courses.Queries.GetCoursesSessionsMaterials;
using CourseManagement.Application.Courses.Queries.GetRecommendedCourses;
using CourseManagement.Application.Courses.Queries.SearchCoursesForEligibilities;
using CourseManagement.Contracts.Courses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.Api.Controllers
{
    [Route("[controller]")]
    public class CoursesController(ISender _mediator) : ApiController
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCourse([FromBody] CreateCourseRequest request)
        {
            request.Eligibility.Subject = request.Subject;

            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            List<(string, int, int)> addedSkills = [];

            if (request.Skills != null)
            {
                foreach (var skill in request.Skills)
                {
                    (string, int, int) addedSkill = (skill.SkillName, skill.LevelCap, skill.Weight);

                    addedSkills.Add(addedSkill);
                }
            }
            
            var command = new CreateCourseCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                subject: request.Subject,
                description: request.Description,
                position: request.Eligibility.Position,
                positionIds: request.Eligibility.PositionIds,
                department: request.Eligibility.Department,
                departmentIds: request.Eligibility.DepartmentIds,
                job: request.Eligibility.Job,
                jobIds: request.Eligibility.JobIds,
                newSkills: addedSkills);

            var addCourseResult = await _mediator.Send(command);

            return addCourseResult.Match(
                course => CreatedAtAction(
                    nameof(GetCourse),
                    new CourseResponse(
                        CourseId: course.CourseId,
                        Subject: course.Subject,
                        Description: course.Description)),
                Problem);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> EditCourse(EditCourseRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new UpdateCourseCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                oldCourseSubject: request.OldSubject,
                subject: request.Subject,
                description: request.Description);

            var updateCourseResult = await _mediator.Send(command);

            return updateCourseResult.Match(
                course => CreatedAtAction(
                    nameof(GetCourse),
                    new CourseResponse(
                        CourseId: course.CourseId,
                        Subject: course.Subject,
                        Description: course.Description)),
                Problem);
        }

        [Authorize]
        [HttpGet("Course")]
        public async Task<IActionResult> GetCourse(string Subject)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new GetCourseQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                subject: Subject);

            var getCourseResult = await _mediator.Send(query);

            return getCourseResult.Match(
                courses => Ok(
                    courses.ConvertAll(
                        course => new CourseResponse(
                            CourseId: course.CourseId,
                            Subject: course.Subject,
                            Description: course.Description))),
                Problem);
        }

        [Authorize]
        [HttpGet("Paginated")]
        public async Task<IActionResult> ListPaginatedCourses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new PaginatedGetAllCoursesQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                pageNumber: pageNumber,
                pageSize: pageSize);

            var getAllCoursesResult = await _mediator.Send(query);

            return getAllCoursesResult.Match(
                paged => Ok(new
                {
                    paged.PageNumber,
                    paged.PageSize,
                    paged.TotalCount,
                    paged.TotalPages,
                    Items = paged.Items.Select(c => new CourseResponse(
                        CourseId: c.CourseId,
                        Subject: c.Subject,
                        Description: c.Description
                    ))
                }),
                Problem);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListCourses()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetAllCoursesQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var getCourseResult = await _mediator.Send(query);

            return getCourseResult.Match(
                courses => Ok(
                    courses.ConvertAll(
                        course => new CourseResponse(
                            CourseId: course.CourseId,
                            Subject: course.Subject,
                            Description: course.Description))),
                Problem);
        }

        [Authorize]
        [HttpPut("Eligibility")] 
        public async Task<IActionResult> AddCourseEligibilities([FromBody] AddCourseEligibilityRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new AddCourseEligibilityCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                courseSubject: request.Subject,
                position: request.Position,
                positionIds: request.PositionIds,
                department: request.Department,
                departmentIds: request.DepartmentIds,
                job: request.Job,
                jobIds: request.JobIds);

            var addCourseEligibilityResult = await _mediator.Send(command);

            return addCourseEligibilityResult.Match(
                eligibilities => CreatedAtAction(
                    nameof(GetCourseEligibilities),
                    eligibilities.ConvertAll(
                        eligibility => new EligibilityResponse(
                            EligibilityId: eligibility.EligibilityId,
                            Course: eligibility.Course,
                            Key: eligibility.Key,
                            Value: eligibility.Value))),
                Problem);
        }

        [Authorize]
        [HttpGet("Eligibilities")]
        public async Task<IActionResult> GetCourseEligibilities(string Subject)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new GetCourseEligibilitiesQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                subject: Subject);

            var getCourseEligibilitiesResult = await _mediator.Send(query);

            return getCourseEligibilitiesResult.Match(
                eligibilities => Ok(
                    eligibilities.ConvertAll(
                        eligibility => new EligibilityValuesResponse(
                            EligibilityId: eligibility.EligibilityId,
                            Course: eligibility.Course,
                            Key: eligibility.Key,
                            Value: eligibility.Value))),
                Problem);
        }

        [Authorize]
        [HttpGet("Recommendation")]
        public async Task<IActionResult> SeeRecommendedCourses(string Alias)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetRecommendedCoursesQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                alias: Alias);

            var recommendedCoursesResult = await _mediator.Send(query);

            return recommendedCoursesResult.Match(
                courses => Ok(
                    courses.ConvertAll(
                        course => new CourseResponse(
                            CourseId: course.CourseId,
                            Subject: course.Subject,
                            Description: course.Description))),
                Problem);
        }

        [Authorize]
        [HttpPost("Session")]
        public async Task<IActionResult> AddCourseSession(AddCourseSessionRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new AddCourseSessionCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                sessionName: request.Name,
                subject: request.Subject,
                startDate: request.StartDate,
                endDate: request.EndDate,
                trainerId: request.TrainerId,
                isOffline: request.IsOffline,
                seats: request.Seats,
                link: request.Link,
                app: request.App);

            var addCourseSessionResult = await _mediator.Send(command);

            return addCourseSessionResult.Match(
                session => CreatedAtAction(
                    nameof(GetCourseSessions),
                    new SessionResponse(
                        SessionId: session.SessionId,
                        SessionName: session.Name,
                        CourseId: session.CourseId,
                        CourseSubject: session.CourseSubject,
                        StartDate: session.StartDate,
                        EndDate: session.EndDate,
                        TrainerId: session.TrainerId,
                        TrainerAlias: session.TrainerAlias,
                        IsOffline: session.IsOffline,
                        Seats: session.Seats,
                        Link: session.Link,
                        App: session.App)),
                Problem);
        }

        [Authorize]
        [HttpGet("Sessions")]
        public async Task<IActionResult> GetCourseSessions(string Subject)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetCourseSessionsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                subject: Subject);

            var getCourseSessionsResult = await _mediator.Send(query);

            return getCourseSessionsResult.Match(
                results => Ok(
                    results.ConvertAll(
                        result => new SessionResponse(
                            SessionId: result.CourseSession.Id,
                            SessionName: result.CourseSession.Name,
                            CourseId: result.CourseSession.CourseId,
                            CourseSubject: result.CourseSubject,
                            StartDate: result.CourseSession.StartDate,
                            EndDate: result.CourseSession.EndDate,
                            TrainerId: result.CourseSession.TrainerId,
                            TrainerAlias: result.TrainerName,
                            IsOffline: result.CourseSession.IsOffline,
                            Seats: result.CourseSession.Seats,
                            Link: result.CourseSession.Link,
                            App: result.CourseSession.App))),
                Problem);
        }

        [Authorize]
        [HttpGet("Eligibilities/User")]
        public async Task<IActionResult> GetUserEligibleCourses(int userId)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetCoursesForEligibilitiesQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                userId: userId);

            var getUserEligibilitiesForCourseResult = await _mediator.Send(query);

            return getUserEligibilitiesForCourseResult.Match(
                courses => Ok(
                    courses.ConvertAll(
                        course => new CourseResponse(
                            CourseId: course.CourseId,
                            Subject: course.Subject,
                            Description: course.Description))),
                Problem);
        }

        [Authorize]
        [HttpGet("Eligibilities/Admin/User")]
        public async Task<IActionResult> AdminGetUserEligibleCourses(int userId)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetCoursesForEligibilitiesAdminQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                userId: userId);

            var getUserEligibilitiesForCourseResult = await _mediator.Send(query);

            return getUserEligibilitiesForCourseResult.Match(
                courses => Ok(
                    courses.ConvertAll(
                        course => new CourseResponse(
                            CourseId: course.CourseId,
                            Subject: course.Subject,
                            Description: course.Description))),
                Problem);
        }

        [Authorize]
        [HttpGet("Eligibilities/User/Subject")]
        public async Task<IActionResult> SearchUserEligibleCourses([FromQuery] int userId, [FromQuery] string subject)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new SearchCourseForEligibilitiesQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                userId: userId,
                subject: subject);

            var searchCoursesFromUserEligibleCoursesResult = await _mediator.Send(query);

            return searchCoursesFromUserEligibleCoursesResult.Match(
                courses => Ok(
                    courses.ConvertAll(
                        course => new CourseResponse(
                            CourseId: course.CourseId,
                            Subject: course.Subject,
                            Description: course.Description))),
                Problem);
        }

        [Authorize]
        [HttpGet("Eligibilities/Admin/User/Subject")]
        public async Task<IActionResult> AdminSearchUserEligibleCourses([FromQuery] int userId, [FromQuery] string subject)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new SearchCourseForEligibilitiesAdminQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                userId: userId,
                subject: subject);

            var adminSearchCourseFromUsersEligibleCoursesResult = await _mediator.Send(query);

            return adminSearchCourseFromUsersEligibleCoursesResult.Match(
                courses => Ok(
                    courses.ConvertAll(
                        course => new CourseResponse(
                            CourseId: course.CourseId,
                            Subject: course.Subject,
                            Description: course.Description))),
                Problem);
        }

        [Authorize]
        [HttpPost("Session/Material")]
        public async Task<IActionResult> AddMaterial(AddCourseSessionMaterialRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new AddCourseSessionMaterialCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                courseId: request.CourseId,
                sessionId: request.SessionId,
                path: request.Path,
                isVideo: request.IsVideo);

            var addedCourseSessionMaterialResult = await _mediator.Send(command);

            List<MaterialResponse>? Materials = [];

            if (addedCourseSessionMaterialResult.IsError == false)
            {
                addedCourseSessionMaterialResult.Value.Materials.ForEach(m =>
                    Materials.Add(new MaterialResponse(
                        MaterialId: m.MaterialId,
                        SessionId: m.SessionId,
                        SessionName: m.SessionName,
                        CourseId: m.CourseId,
                        CourseSubject: m.CourseSubject,
                        Path: m.Path,
                        IsVideo: m.IsVideo)));
            }

            return addedCourseSessionMaterialResult.Match(
                session => CreatedAtAction(
                    nameof(GetCourseSessions),
                    new SessionAndMaterialsResponse(
                        SessionId: session.SessionId,
                        Name: session.Name,
                        CourseId: session.CourseId,
                        StartDate: session.StartDate,
                        EndDate: session.EndDate,
                        IsOffline: session.IsOffline,
                        Materials: Materials)),
                Problem);
        }

        [Authorize]
        [HttpPut("Session/Material")]
        public async Task<IActionResult> UpdateMaterial(UpdateCourseSessionMaterialRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new UpdateCourseSessionMaterialCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                courseId: request.CourseId,
                sessionId: request.SessionId,
                materialId: request.MaterialId,
                path: request.Path,
                isVideo: request.IsVideo);

            var updateCourseSessionMaterialResult = await _mediator.Send(command);

            List<MaterialResponse>? Materials = [];

            if (updateCourseSessionMaterialResult.IsError == false)
            {
                updateCourseSessionMaterialResult.Value.Materials.ForEach(m =>
                    Materials.Add(new MaterialResponse(
                        MaterialId: m.MaterialId,
                        SessionId: m.SessionId,
                        SessionName: m.SessionName,
                        CourseId: m.CourseId,
                        CourseSubject: m.CourseSubject,
                        Path: m.Path,
                        IsVideo: m.IsVideo)));
            }

            return updateCourseSessionMaterialResult.Match(
                session => CreatedAtAction(
                    nameof(GetCourseSessions),
                    new SessionAndMaterialsResponse(
                        SessionId: session.SessionId,
                        Name: session.Name,
                        CourseId: session.CourseId,
                        StartDate: session.StartDate,
                        EndDate: session.EndDate,
                        IsOffline: session.IsOffline,
                        Materials: Materials)),
                Problem);
        }

        [Authorize]
        [HttpPut("Session/MaterialPlacement")]
        public async Task<IActionResult> UpdateMaterialPlacement(UpdateCourseSessionMaterialPlacementRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new UpdateCourseSessionMaterialPlacementCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                oldMaterialId: request.OldMaterialId,
                oldSessionId: request.OldSessionId,
                oldCourseId: request.OldCourseId,
                newMaterialSessionName: request.NewMaterialSessionName,
                newCourseId: request.NewCourseId);

            var updateCourseSessionMaterialPlacementResult = await _mediator.Send(command);

            List<MaterialResponse>? Materials = [];

            if (updateCourseSessionMaterialPlacementResult.IsError == false)
            {
                updateCourseSessionMaterialPlacementResult.Value.Materials.ForEach(m =>
                    Materials.Add(new MaterialResponse(
                        MaterialId: m.MaterialId,
                        SessionId: m.SessionId,
                        SessionName: m.SessionName,
                        CourseId: m.CourseId,
                        CourseSubject: m.CourseSubject,
                        Path: m.Path,
                        IsVideo: m.IsVideo)));
            }

            return updateCourseSessionMaterialPlacementResult.Match(
                session => CreatedAtAction(
                    nameof(GetCourseSessions),
                    new SessionAndMaterialsResponse(
                        SessionId: session.SessionId,
                        Name: session.Name,
                        CourseId: session.CourseId,
                        StartDate: session.StartDate,
                        EndDate: session.EndDate,
                        IsOffline: session.IsOffline,
                        Materials: Materials)),
                Problem);
        }

        [Authorize]
        [HttpGet("Course/Session/Materials")]
        public async Task<IActionResult> GetCourseSessionMaterials([FromQuery] int CourseId, [FromQuery] int SessionId)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetCourseSessionMaterialsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                courseId: CourseId,
                sessionId: SessionId);

            var getCourseSessionMaterials = await _mediator.Send(query);

            return getCourseSessionMaterials.Match(
                materials => Ok(
                    materials.ConvertAll(
                        material => new MaterialResponse(
                            MaterialId: material.MaterialId,
                            SessionId: material.SessionId,
                            SessionName: material.SessionName,
                            CourseId: material.CourseId,
                            CourseSubject: material.CourseSubject,
                            Path: material.Path,
                            IsVideo: material.IsVideo))),
                Problem);
        }

        [Authorize]
        [HttpGet("Sessions/Materials")]
        public async Task<IActionResult> GetCoursesWithSessionsAndMaterials()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetCoursesSessionsMaterialsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var getCoursesWithSessionsAndMaterialsResult = await _mediator.Send(query);

            List<CourseSessionsMaterialsResponse> coursesSessionsAndMaterials = getCoursesWithSessionsAndMaterialsResult.IsError == false ?
                getCoursesWithSessionsAndMaterialsResult.Value.Select(c => new CourseSessionsMaterialsResponse(
                    CourseId: c.CourseId,
                    Subject: c.Subject,
                    Description: c.Description,
                    SessionsAndMaterials: c.Sessions != null ?
                        c.Sessions.Select(s => new SessionAndMaterialsResponse(
                            SessionId: s.Id,
                            Name: s.Name,
                            CourseId: s.CourseId,
                            StartDate: s.StartDate,
                            EndDate: s.EndDate,
                            IsOffline: s.IsOffline,
                            Materials: s.Materials != null ?
                                s.Materials.Select(m => new MaterialResponse(
                                    MaterialId: m.Id,
                                    SessionId: m.SessionId,
                                    SessionName: s.Name,
                                    CourseId: s.CourseId,
                                    CourseSubject: c.Subject,
                                    Path: m.Path,
                                    IsVideo: m.IsVideo))
                                .ToList() : []))
                        .ToList() : []))
                .ToList() : [];
            
            return getCoursesWithSessionsAndMaterialsResult.Match(
                courses => Ok(coursesSessionsAndMaterials),
                Problem);
        }

        [Authorize]
        [HttpGet("Course/Session/Material/Download")]
        public async Task<IActionResult> DownloadMaterial(
            [FromQuery] int UserId,
            [FromQuery] int CourseId,
            [FromQuery] int SessionId,
            [FromQuery] int MaterialId)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new DownloadCourseSessionMaterialQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                userId: UserId,
                courseId: CourseId,
                sessionId: SessionId,
                materialId: MaterialId);

            var result = await _mediator.Send(query);

            return result.Match(
                fileInfo => PhysicalFile(
                    physicalPath: fileInfo.Path,
                    contentType: "application/octet-stream",
                    fileDownloadName: fileInfo.FileName,
                    enableRangeProcessing: true),
                Problem
            );
        }
    }
}

/*
using CourseManagement.Application.Courses.Common.Dto;

        private IActionResult StreamFileWithRanges(DownloadMaterialFileInfo fileInfo)
        {
            var filePath = fileInfo.Path;

            // This check in its own, should be in infrastructure.
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found on disk.");
            }
            
            var fileName = Path.GetFileName(filePath);
            var contentType = "application/octet-stream";

            // Let ASP.NET Core handle ranges automatically
            return PhysicalFile(filePath, contentType, fileName, enableRangeProcessing: true);
        }
*/
