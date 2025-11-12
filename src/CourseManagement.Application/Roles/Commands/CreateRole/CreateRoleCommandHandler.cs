using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Roles.Common.Dto;
using CourseManagement.Domain.Roles;
//using CourseManagement.Application.Roles.Commands.AddRolePrivilege;

namespace CourseManagement.Application.Roles.Commands.CreateRole
{
    public class CreateRoleCommandHandler(
        IRolesRepository rolesRepository,
        //ISender mediator,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<CreateRoleCommand, ErrorOr<RoleDto>>
    {
        private readonly IRolesRepository _rolesRepository = rolesRepository;
        //private readonly ISender _mediator = mediator;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<RoleDto>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (await _rolesRepository.RoleTypeExistsAsync(command.roleType) == true)
                {
                    return Error.Validation(description: "Role already added.");
                }

                var role = new Role(
                    roleType: command.roleType);

                if (role.CheckIfRoleIsValid() != Result.Success)
                {
                    return role.CheckIfRoleIsValid().Errors;
                }

                await _rolesRepository.AddRoleAsync(role);

                await _unitOfWork.CommitChangesAsync();

                //var addRolePrivilegesCommand = new AddRolePrivilegesCommand(
                //    RoleId: role.Id,
                //    PrivilegeIds: command.privilegeIds);

                //var addRolePrivilegesResult = await _mediator.Send(addRolePrivilegesCommand);

                //if (addRolePrivilegesResult.IsError)
                //{
                //    await _rolesRepository.RemoveRoleAsync(role.Id);
                //    return addRolePrivilegesResult.Errors;
                //}

                RoleDto roleDto = new RoleDto
                {
                    RoleId = role.Id,
                    RoleType = role.RoleType
                };

                roleDto.AffectedId = role.Id;

                return roleDto;
            }
            catch (Exception ex)
            {
                return Error.Failure($"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
