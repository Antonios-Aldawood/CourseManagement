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

namespace CourseManagement.Application.Roles.Commands.AddRolePrivileges
{
    public class AddRolePrivilegesCommandHandler(
        IRolesRepository rolesRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<AddRolePrivilegesCommand, ErrorOr<RoleDto>>
    {
        private readonly IRolesRepository _rolesRepository = rolesRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<RoleDto>> Handle(AddRolePrivilegesCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (await _rolesRepository.GetByIdAsync(command.roleId) is not Role role)
                {
                    return Error.Validation(description: "Role not found.");
                }

                foreach (var privilegeId in command.privilegeIds.Distinct())
                {
                    var exists = await _rolesRepository.PrivilegeExistsAsync(privilegeId);

                    if (!exists)
                    {
                        return Error.Validation(description: $"Privilege {privilegeId} not found.");
                    }

                    var rolePrivilege = new RolesPrivileges(
                        command.roleId,
                        privilegeId);

                    if (await _rolesRepository.AddRolesPrivilegesAsync(rolePrivilege) == false)
                    {
                        return Error.Validation(description: "Role already has privilege.");
                    }
                }

                await _unitOfWork.CommitChangesAsync();

                RoleDto roleResponse = RoleDto.AddDto(role);

                return roleResponse;
            }
            catch (Exception ex)
            {
                return Error.Failure($"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}

/*
//This may solve the multiple checking visits to teh DB:

var role = await (from role in _dbContext.Roles
                  where (from rp in _dbContext.RolesPrivileges
                         where rp.RoleId == role.Id && privilegeIds.Contains(rp.PrivilegeId)
                         select rp.PrivilegeId)
                         .Distinct()
                         .Count() == privilegeIds.Count
                  select role)
                  .FirstOrDefaultAsync();
*/