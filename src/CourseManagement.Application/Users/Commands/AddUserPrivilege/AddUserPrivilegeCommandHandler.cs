using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;
using CourseManagement.Domain.Users;
using CourseManagement.Application.Roles.Commands.CreateRole;
using CourseManagement.Domain.Roles;

namespace CourseManagement.Application.Users.Commands.AddUserPrivilege
{
    public class AddUserPrivilegeCommandHandler(
        IUsersRepository usersRepository,
        IRolesRepository rolesRepository,
        ISender mediator,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<AddUserPrivilegeCommand, ErrorOr<UserRoleDto>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly IRolesRepository _rolesRepository = rolesRepository;
        private readonly ISender _mediator = mediator;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<UserRoleDto>> Handle(AddUserPrivilegeCommand command, CancellationToken cancellationToken)
        {
            var createUserNewRoleCommand = new CreateRoleCommand(
                ipAddress: command.ipAddress,
                headers: command.headers,
                roleType: command.newRole);

            var createUserNewRoleResult = await _mediator.Send(createUserNewRoleCommand);

            if (createUserNewRoleResult.IsError)
            {
                return createUserNewRoleResult.Errors;
            }

            try
            {
                if (await _usersRepository.GetByIdAsync(command.userId) is not User user)
                {
                    if (createUserNewRoleResult.IsError == false)
                    {
                        await _rolesRepository.RemoveRoleAsync(createUserNewRoleResult.Value.RoleId);
                    }

                    return Error.Validation(description: "User not found.");
                }

                foreach (var privilegeId in command.privilegeIds.Distinct())
                {
                    if (await _rolesRepository.PrivilegeExistsAsync(privilegeId) == false)
                    {
                        if (createUserNewRoleResult.IsError == false)
                        {
                            await _rolesRepository.RemoveRoleAsync(createUserNewRoleResult.Value.RoleId);
                        }

                        return Error.Validation(description: "Privilege not found.");
                    }

                    //There has got to be a way to put these in a list and use it instead of visiting DB again.
                    //That way we lessen the current 5 visits (outside of validation), to the required 4 visits.
                    if (await _rolesRepository.RolePrivilegeExistsAsync(user.RoleId, privilegeId))
                    {
                        if (createUserNewRoleResult.IsError == false)
                        {
                            await _rolesRepository.RemoveRoleAsync(createUserNewRoleResult.Value.RoleId);
                        }

                        return Error.Validation(description: "User's role already has privilege.");
                    }
                }

                var oldRolePrivileges = await _rolesRepository.GetPrivilegesByRoleId(user.RoleId);

                user.RoleId = createUserNewRoleResult.Value.RoleId;

                foreach (Privilege privilege in oldRolePrivileges)
                {
                    RolesPrivileges addOldRolePrivilegesToNewRole = new RolesPrivileges(
                        roleId: user.RoleId,
                        privilegeId: privilege.Id);

                    await _rolesRepository.AddRolesPrivilegesAsync(addOldRolePrivilegesToNewRole);
                }

                foreach (var privilegeId in command.privilegeIds)
                {
                    RolesPrivileges newRolePrivilege = new RolesPrivileges(
                        roleId: user.RoleId,
                        privilegeId: privilegeId);

                    await _rolesRepository.AddRolesPrivilegesAsync(newRolePrivilege);
                }

                await _unitOfWork.CommitChangesAsync();

                UserRoleDto userDto = new UserRoleDto
                {
                    Id = user.Id,
                    Alias = user.Alias,
                    Role = createUserNewRoleResult.Value.RoleType
                };

                userDto.AffectedId = user.Id;

                return userDto;
            }
            catch (Exception ex)
            {
                if (createUserNewRoleResult.IsError == false)
                {
                    await _rolesRepository.RemoveRoleAsync(createUserNewRoleResult.Value.RoleId);
                }

                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
