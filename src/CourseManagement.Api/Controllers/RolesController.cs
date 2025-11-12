using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http.Extensions;
using CourseManagement.Contracts.Roles;
using CourseManagement.Application.Roles.Commands.CreateRole;
using CourseManagement.Application.Roles.Queries.GetRole;
using CourseManagement.Application.Roles.Queries.GetAllRoles;
using CourseManagement.Application.Roles.Queries.GetRolePrivileges;
using CourseManagement.Application.Roles.Commands.AddRolePrivileges;
using CourseManagement.Application.Roles.Queries.GetAllPrivileges;

namespace CourseManagement.Api.Controllers
{
    [Route("[controller]")]
    public class RolesController(ISender _mediator) : ApiController
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddRole(CreateRoleRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var command = new CreateRoleCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                roleType: request.RoleType);

            var addRoleResult = await _mediator.Send(command);

            return addRoleResult.Match(
                role => CreatedAtAction(
                    nameof(GetRole),
                    new RoleResponse(
                        RoleId: role.RoleId,
                        RoleType: role.RoleType)),
                Problem);
        }

        [Authorize]
        [HttpGet("Role")]
        public async Task<IActionResult> GetRole(string Role)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new GetRoleQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                roleType: Role);

            var getRoleResult = await _mediator.Send(query);

            return getRoleResult.Match(
                roles => Ok(
                    roles.ConvertAll(
                        role => new RoleResponse(
                            RoleId: role.RoleId,
                            RoleType: role.RoleType))),
                Problem);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListRoles()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new GetAllRolesQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var listRolesResult = await _mediator.Send(query);

            return listRolesResult.Match(
                roles => Ok(
                    roles.ConvertAll(
                        role => new RoleResponse(
                            RoleId: role.RoleId,
                            RoleType: role.RoleType))),
                Problem);
        }

        [Authorize]
        [HttpGet("Privileges/Role")]
        public async Task<IActionResult> GetRolePrivileges(string RoleType)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new GetRolePrivilegesQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                RoleType);

            var GetPrivilegesByRoleResult = await _mediator.Send(query);

            return GetPrivilegesByRoleResult.Match(
                privileges => Ok(
                    privileges.ConvertAll(
                        privilege => new PrivilegeResponse(
                            PrivilegeId: privilege.PrivilegeId,
                            PrivilegeName: privilege.PrivilegeName))),
                Problem);
        }

        [Authorize]
        [HttpPost("assign-privileges")]
        public async Task<IActionResult> AssignPrivilegesToRole(
            [FromBody] AssignRolePrivilegesRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var command = new AddRolePrivilegesCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                roleId: request.RoleId,
                privilegeIds: request.PrivilegeIds);

            var result = await _mediator.Send(command);

            return result.Match(
                role => NoContent(
                    //Ok(
                    //  new RoleResponse(
                    //    RoleId: role.RoleId,
                    //    RoleType: role.RoleType)),
                    ),
                Problem);
        }


        [Authorize]
        [HttpGet("AllPrivileges")]
        public async Task<IActionResult> ListPrivileges()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new GetAllPrivilegesQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var listPrivilegesResult = await _mediator.Send(query);

            return listPrivilegesResult.Match(
                privileges => Ok(
                    privileges.ConvertAll(
                        privilege => new PrivilegeResponse(
                            PrivilegeId: privilege.PrivilegeId,
                            PrivilegeName: privilege.PrivilegeName))),
                Problem);
        }
    }
}
