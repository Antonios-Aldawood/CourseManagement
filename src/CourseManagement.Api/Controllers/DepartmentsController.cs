using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using CourseManagement.Contracts.Departments;
using CourseManagement.Application.Departments.Commands.CreateDepartment;
using CourseManagement.Application.Departments.Commands.UpdateDepartment;
using CourseManagement.Application.Departments.Queries.FilterDepartmentsByMembers;
using CourseManagement.Application.Departments.Queries.GetAllDepartments;
using CourseManagement.Application.Departments.Queries.GetDepartment;
using CourseManagement.Application.Departments.Queries.SortDepartmentsByLeastMembers;
using CourseManagement.Application.Departments.Queries.SortDepartmentsByMostMembers;

namespace CourseManagement.Api.Controllers
{
    [Route("[controller]")]
    public class DepartmentsController(ISender _mediator) : ApiController
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddDepartment(CreateDepartmentRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var command = new CreateDepartmentCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                name: request.Name,
                minMembers: request.MinMembers,
                maxMembers: request.MaxMembers,
                description: request.Description);

            var addDepartmentResult = await _mediator.Send(command);

            return addDepartmentResult.Match(
                department => CreatedAtAction(
                    nameof(GetDepartment),
                    new DepartmentResponse(
                        DepartmentId: department.DepartmentId,
                        Name: department.Name,
                        MinMembers: department.MinMembers,
                        MaxMembers: department.MaxMembers,
                        Description: department.Description)),
                Problem);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> EditDepartment(EditDepartmentRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new UpdateDepartmentCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                oldDepartmentName: request.OldName,
                name: request.Name,
                minMembers: request.MinMembers,
                maxMembers: request.MaxMembers,
                description: request.Description);

            var editDepartmentResult = await _mediator.Send(command);

            return editDepartmentResult.Match(
                department => CreatedAtAction(
                    nameof(GetDepartment),
                    new DepartmentResponse(
                        DepartmentId: department.DepartmentId,
                        Name: department.Name,
                        MinMembers: department.MinMembers,
                        MaxMembers: department.MaxMembers,
                        Description: department.Description)),
                Problem);
        }

        [Authorize]
        [HttpGet("Department")]
        public async Task<IActionResult> GetDepartment(string Name)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new GetDepartmentQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                name: Name);

            var getDepartmentResult = await _mediator.Send(query);

            return getDepartmentResult.Match(
                departments => Ok(
                    departments.ConvertAll(
                        department => new DepartmentResponse(
                            DepartmentId: department.DepartmentId,
                            Name: department.Name,
                            MinMembers: department.MinMembers,
                            MaxMembers: department.MaxMembers,
                            Description: department.Description))),
                Problem);
        }

        [Authorize]
        [HttpGet("Paginated")]
        public async Task<IActionResult> ListPaginatedDepartments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new PaginatedGetAllDepartmentsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                pageNumber: pageNumber,
                pageSize: pageSize);

            var listDepartmentsResult = await _mediator.Send(query);

            return listDepartmentsResult.Match(
                paged => Ok(new
                {
                    paged.PageNumber,
                    paged.PageSize,
                    paged.TotalCount,
                    paged.TotalPages,
                    Items = paged.Items.Select(d => new DepartmentResponse(
                        DepartmentId: d.DepartmentId,
                        Name: d.Name,
                        MinMembers: d.MinMembers,
                        MaxMembers: d.MaxMembers,
                        Description: d.Description
                    ))
                }),
                Problem);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListDepartments()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetAllDepartmentsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var getDepartmentResult = await _mediator.Send(query);

            return getDepartmentResult.Match(
                departments => Ok(
                    departments.ConvertAll(
                        department => new DepartmentResponse(
                            DepartmentId: department.DepartmentId,
                            Name: department.Name,
                            MinMembers: department.MinMembers,
                            MaxMembers: department.MaxMembers,
                            Description: department.Description))),
                Problem);
        }

        [Authorize]
        [HttpGet("MemberFilter")]
        public async Task<IActionResult> FilterDepartmentsMembers(int Members)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new FilterDepartmentsByMembersQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                members: Members);

            var filterResult = await _mediator.Send(query);

            return filterResult.Match(
                departments => Ok(
                    departments.ConvertAll(
                        department => new DepartmentResponse(
                            DepartmentId: department.DepartmentId,
                            Name: department.Name,
                            MinMembers: department.MinMembers,
                            MaxMembers: department.MaxMembers,
                            Description: department.Description))),
                Problem);
        }

        [Authorize]
        [HttpGet("SortLeastMembers")]
        public async Task<IActionResult> SortDepartmentsLeastMembers()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new SortDepartmentsByLeastMembersQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var sortResult = await _mediator.Send(query);

            return sortResult.Match(
                departments => Ok(
                    departments.ConvertAll(
                        department => new DepartmentResponse(
                            DepartmentId: department.DepartmentId,
                            Name: department.Name,
                            MinMembers: department.MinMembers,
                            MaxMembers: department.MaxMembers,
                            Description: department.Description))),
                Problem);
        }

        [Authorize]
        [HttpGet("SortMostMembers")]
        public async Task<IActionResult> SortDepartmentsMostMembers()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new SortDepartmentsByMostMembersQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var sortResult = await _mediator.Send(query);

            return sortResult.Match(
                departments => Ok(
                    departments.ConvertAll(
                        department => new DepartmentResponse(
                            DepartmentId: department.DepartmentId,
                            Name: department.Name,
                            MinMembers: department.MinMembers,
                            MaxMembers: department.MaxMembers,
                            Description: department.Description))),
                Problem);
        }
    }
}
