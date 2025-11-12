using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http.Extensions;
using CourseManagement.Contracts.Users;
using CourseManagement.Application.Users.Commands.LoginUser;
using CourseManagement.Application.Users.Commands.RefreshUserToken;
using CourseManagement.Application.Users.Commands.RevokeUserToken;
using CourseManagement.Application.Users.Commands.VerifyUser;
using CourseManagement.Application.Users.Commands.CreateUser;
using CourseManagement.Application.Users.Queries.GetUserProfile;
using CourseManagement.Application.Users.Queries.GetUser;
using CourseManagement.Application.Users.Queries.GetAllUsers;
using CourseManagement.Application.Users.Commands.AddUserPrivilege;
using CourseManagement.Application.Users.Queries.FilterUsersByJob;
using CourseManagement.Application.Users.Queries.FilterUsersByPosition;
using CourseManagement.Application.Users.Queries.FilterUsersByRole;
using CourseManagement.Application.Users.Queries.FilterUsersByUpper1;
using CourseManagement.Application.Users.Queries.FilterUsersByUpper2;
using CourseManagement.Application.Users.Queries.GetAllAddresses;
using CourseManagement.Application.Users.Queries.GetAllUsersPositions;
using CourseManagement.Application.Users.Queries.GetAvailableUppers;
using CourseManagement.Application.Users.Queries.GetPrivilegesByUser;
using CourseManagement.Application.Users.Queries.GetAllTrainers;
using System.Security.Claims;

namespace CourseManagement.Api.Controllers
{
    [Route("[controller]")]
    public class UsersController(
        ISender _mediator) : ApiController
    {

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> UserLogin(UserLoginRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            //var url = Request.GetDisplayUrl().ToString();

            var command = new LoginUserCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                email: request.Email,
                password: request.Password);

            var loginResult = await _mediator.Send(command);

            return loginResult.Match(
                response => Ok(response),
                Problem);
        }

        [AllowAnonymous]
        [HttpPost("Refresh")]
        public async Task<IActionResult> RefreshTheUserRefreshToken([FromBody] RefreshRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var command = new RefreshUserTokenCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                refreshToken: request.RefreshToken);

            var refreshUserTokenResult = await _mediator.Send(command);

            return refreshUserTokenResult.Match(
                tokens => Ok(new RefreshResponse(
                    token: tokens.Token,
                    refreshToken: tokens.RefreshToken,
                    refreshTokenExpiry: tokens.RefreshTokenExpiryDate)),
                Problem);
        }

        [Authorize]
        [HttpPost("Revoke")]
        public async Task<IActionResult> RevokeTheUserRefreshToken()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            var command = new RevokeUserTokenCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                userId: userId);

            var revokeUserTokenResult = await _mediator.Send(command);

            return revokeUserTokenResult.Match(
                success => NoContent(),
                Problem);
        }

        [Authorize]
        [HttpPost("Verify")]
        public async Task<IActionResult> VerifyUser(UserVerifyRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            if (request.ConfirmPassword != request.NewPassword)
            {
                return ValidationProblem("Password wasn't confirmed correctly.");
            }

            var command = new VerifyUserCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                alias: request.Alias,
                verificationCode: request.Code,
                newPassword: request.NewPassword
            );

            var result = await _mediator.Send(command);

            return result.Match(
                user => Ok(new UserShortResponse(
                    Id: user.Id,
                    Alias: user.Alias)),
                Problem);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserCreateRequest request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            if (request.ConfirmPassword != request.Password)
            {
                return BadRequest("Password wasn't confirmed correctly.");
            }

            var command = new CreateUserCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                alias: request.Alias,
                email: request.Email,
                password: request.Password,
                phoneNumber: request.PhoneNumber,
                position: request.Position,
                roleType: request.RoleType,
                city: request.City,
                region: request.Region,
                road: request.Road,
                title: request.JobTitle,
                upper: request.Upper,
                agreedSalary: request.AgreedSalary);

            var createUserResponse = await _mediator.Send(command);

            return createUserResponse.Match(
                user => CreatedAtAction(
                    nameof(GetUserProfile),
                    new UserShortResponse(
                        Id: user.Id,
                        Alias: user.Alias)),
                Problem);
        }

        [Authorize]
        [HttpGet("Profile")]
        public async Task<IActionResult> GetUserProfile(string Alias)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new GetUserProfileQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                alias: Alias);

            var getUserProfileResult = await _mediator.Send(query);

            return getUserProfileResult.Match(
                user => Ok(
                    new UserProfileResponse(
                        Id: user.Id,
                        Alias: user.Alias,
                        Email: user.Email,
                        PhoneNumber: user.PhoneNumber,
                        Position: user.Position,
                        Upper1Alias: user.Upper1!.Alias,
                        Upper2Alias: user.Upper2!.Alias)),
                Problem);
        }

        [Authorize]
        [HttpGet("User")]
        public async Task<IActionResult> GetUser(string Alias)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new GetUserQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                alias: Alias);

            var GetUserResult = await _mediator.Send(query);

            return GetUserResult.Match(
                users => Ok(users.ConvertAll(
                    user => new UserResponse(
                        Id: user.UserId,
                        Alias: user.Alias,
                        Email: user.Email,
                        PhoneNumber: user.PhoneNumber,
                        Position: user.Position,
                        City: user.City,
                        Region: user.Region,
                        Road: user.Road,
                        Role: user.Role,
                        JobTitle: user.JobTitle,
                        JobDescription: user.JobDescription,
                        DepartmentName: user.DepartmentName,
                        Upper1Alias: user.Upper1Alias,
                        Upper2Alias: user.Upper2Alias,
                        IsVerified: user.IsVerified))),
                Problem);
        }

        [Authorize]
        [HttpGet("Paginated")]
        public async Task<IActionResult> ListPaginatedUsers([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new PaginatedGetAllUsersQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                pageNumber: pageNumber,
                pageSize: pageSize);

            var listUsersResult = await _mediator.Send(query);

            return listUsersResult.Match(
                paged => Ok(new
                {
                    paged.PageNumber,
                    paged.PageSize,
                    paged.TotalCount,
                    paged.TotalPages,
                    Items = paged.Items.Select(u => new UserResponse(
                        Id: u.UserId,
                        Alias: u.Alias,
                        Email: u.Email,
                        PhoneNumber: u.PhoneNumber,
                        Position: u.Position,
                        City: u.City,
                        Region: u.Region,
                        Road: u.Road,
                        Role: u.Role,
                        JobTitle: u.JobTitle,
                        JobDescription: u.JobDescription,
                        DepartmentName: u.DepartmentName,
                        Upper1Alias: u.Upper1Alias,
                        Upper2Alias: u.Upper2Alias,
                        IsVerified: u.IsVerified
                    ))
                }),
                Problem);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListUsers()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new GetAllUsersQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var listUsersResult = await _mediator.Send(query);

            return listUsersResult.Match(
                users => Ok(
                    users.ConvertAll(
                        user => new UserResponse(
                            Id: user.UserId,
                            Alias: user.Alias,
                            Email: user.Email,
                            PhoneNumber: user.PhoneNumber,
                            Position: user.Position,
                            City: user.City,
                            Region: user.Region,
                            Road: user.Road,
                            Role: user.Role,
                            JobTitle: user.JobTitle,
                            JobDescription: user.JobDescription,
                            DepartmentName: user.DepartmentName,
                            Upper1Alias: user.Upper1Alias,
                            Upper2Alias: user.Upper2Alias,
                            IsVerified: user.IsVerified))),
                Problem);
        }

        [Authorize]
        [HttpPost("give-privilege")]
        public async Task<IActionResult> AddPrivilegeToUser([FromBody] UserAddPrivilege request)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var result = await _mediator.Send(new AddUserPrivilegeCommand(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                userId: request.UserId,
                newRole: request.NewRole,
                privilegeIds: request.PrivilegeIds));

            return result.Match(
                user => NoContent(
                    //Ok(
                    //  new UserRoleResponse(
                    //    Id: user.Id,
                    //    Alias: user.Alias,
                    //    Role: user.Role)),
                    ),
                Problem);
        }

        [Authorize]
        [HttpGet("JobFilter")]
        public async Task<IActionResult> FilterUserJobs(string Job)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new FilterUsersByJobQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                jobTitle: Job);

            var filterResult = await _mediator.Send(query);

            return filterResult.Match(
                users => Ok(
                    users.ConvertAll(
                        user => new UserResponse(
                            Id: user.UserId,
                            Alias: user.Alias,
                            Email: user.Email,
                            PhoneNumber: user.PhoneNumber,
                            Position: user.Position,
                            City: user.City,
                            Region: user.Region,
                            Road: user.Road,
                            Role: user.Role,
                            JobTitle: user.JobTitle,
                            JobDescription: user.JobDescription,
                            DepartmentName: user.DepartmentName,
                            Upper1Alias: user.Upper1Alias,
                            Upper2Alias: user.Upper2Alias,
                            IsVerified: user.IsVerified))),
                Problem);
        }

        [Authorize]
        [HttpGet("PositionFilter")]
        public async Task<IActionResult> FilterUserPositions(string Position)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new FilterUsersByPositionQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                position: Position);

            var filterResult = await _mediator.Send(query);

            return filterResult.Match(
                users => Ok(
                    users.ConvertAll(
                        user => new UserResponse(
                            Id: user.UserId,
                            Alias: user.Alias,
                            Email: user.Email,
                            PhoneNumber: user.PhoneNumber,
                            Position: user.Position,
                            City: user.City,
                            Region: user.Region,
                            Road: user.Road,
                            Role: user.Role,
                            JobTitle: user.JobTitle,
                            JobDescription: user.JobDescription,
                            DepartmentName: user.DepartmentName,
                            Upper1Alias: user.Upper1Alias,
                            Upper2Alias: user.Upper2Alias,
                            IsVerified: user.IsVerified))),
                Problem);
        }

        [Authorize]
        [HttpGet("RoleFilter")]
        public async Task<IActionResult> FilterUserRoles(string Role)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new FilterUsersByRoleQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                role: Role);

            var filterResult = await _mediator.Send(query);

            return filterResult.Match(
                users => Ok(
                    users.ConvertAll(
                        user => new UserResponse(
                            Id: user.UserId,
                            Alias: user.Alias,
                            Email: user.Email,
                            PhoneNumber: user.PhoneNumber,
                            Position: user.Position,
                            City: user.City,
                            Region: user.Region,
                            Road: user.Road,
                            Role: user.Role,
                            JobTitle: user.JobTitle,
                            JobDescription: user.JobDescription,
                            DepartmentName: user.DepartmentName,
                            Upper1Alias: user.Upper1Alias,
                            Upper2Alias: user.Upper2Alias,
                            IsVerified: user.IsVerified))),
                Problem);
        }

        [Authorize]
        [HttpGet("UpperOneFilter")]
        public async Task<IActionResult> FilterUserUpper(string Upper1Alias)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new FilterUsersByUpper1Query(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                upper1Alias: Upper1Alias);

            var filterResult = await _mediator.Send(query);

            return filterResult.Match(
                users => Ok(
                    users.ConvertAll(
                        user => new UserResponse(
                            Id: user.UserId,
                            Alias: user.Alias,
                            Email: user.Email,
                            PhoneNumber: user.PhoneNumber,
                            Position: user.Position,
                            City: user.City,
                            Region: user.Region,
                            Road: user.Road,
                            Role: user.Role,
                            JobTitle: user.JobTitle,
                            JobDescription: user.JobDescription,
                            DepartmentName: user.DepartmentName,
                            Upper1Alias: user.Upper1Alias,
                            Upper2Alias: user.Upper2Alias,
                            IsVerified: user.IsVerified))),
                Problem);
        }

        [Authorize]
        [HttpGet("UpperTwoFilter")]
        public async Task<IActionResult> FilterUserUpperUpper(string Upper2Alias)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new FilterUsersByUpper2Query(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                upper2Alias: Upper2Alias);

            var filterResult = await _mediator.Send(query);

            return filterResult.Match(
                users => Ok(
                    users.ConvertAll(
                        user => new UserResponse(
                            Id: user.UserId,
                            Alias: user.Alias,
                            Email: user.Email,
                            PhoneNumber: user.PhoneNumber,
                            Position: user.Position,
                            City: user.City,
                            Region: user.Region,
                            Road: user.Road,
                            Role: user.Role,
                            JobTitle: user.JobTitle,
                            JobDescription: user.JobDescription,
                            DepartmentName: user.DepartmentName,
                            Upper1Alias: user.Upper1Alias,
                            Upper2Alias: user.Upper2Alias,
                            IsVerified: user.IsVerified))),
                Problem);
        }

        [Authorize]
        [HttpGet("Addresses")]
        public async Task<IActionResult> GetAllAddresses()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new GetAllAddressesQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var GetAllAddressesResult = await _mediator.Send(query);

            return GetAllAddressesResult.Match(
                addresses => Ok(
                    addresses.ConvertAll(
                        address => new UserAddressResponse(
                            City: address.City,
                            Region: address.Region,
                            Road: address.Road))),
                Problem);
        }

        [Authorize]
        [HttpGet("Positions")]
        public async Task<IActionResult> GetAllPositions()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new GetAllUsersPositionsQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var GetAllPositionsResult = await _mediator.Send(query);

            return GetAllPositionsResult.Match(
                positions => Ok(
                    positions.ConvertAll(
                        position => new UserPositionOrUpperAliasResponse(
                            Content: position))),
                Problem);
        }

        [Authorize]
        [HttpGet("Uppers")]
        public async Task<IActionResult> GetUppersInDepartment(string Title)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetAvailableUppersQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                jobTitle: Title);

            var getUppersInDepartmentResult = await _mediator.Send(query);

            return getUppersInDepartmentResult.Match(
                aliases => Ok(
                    aliases.ConvertAll(
                        alias => new UserPositionOrUpperAliasResponse(
                            Content: alias))),
                Problem);
        }

        [Authorize]
        [HttpGet("Privileges")]
        public async Task<IActionResult> GetUserPrivileges(string Alias)
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            var query = new GetPrivilegesByUserQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary,
                alias: Alias);

            var GetPrivilegesByUserResult = await _mediator.Send(query);

            return GetPrivilegesByUserResult.Match(
                privileges => Ok(
                    privileges.ConvertAll(
                        privilege => privilege)),
                Problem);
        }

        [Authorize]
        [HttpGet("Trainers")]
        public async Task<IActionResult> GetTrainers()
        {
            var headersDictionary = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var query = new GetAllTrainersQuery(
                ipAddress: GetClientIp(),
                headers: headersDictionary);

            var getTrainersResult = await _mediator.Send(query);

            return getTrainersResult.Match(
                trainers => Ok(
                    trainers.ConvertAll(
                        trainer => new UserShortResponse(
                            Id: trainer.Id,
                            Alias: trainer.Alias))),
                Problem);
        }
    }
}
