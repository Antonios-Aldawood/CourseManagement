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

namespace CourseManagement.Application.Roles.Queries.GetRole
{
    public class GetRoleQueryHandler(
        IRolesRepository rolesRepository
        ) : IRequestHandler<GetRoleQuery, ErrorOr<List<RoleDto>>>
    {
        private readonly IRolesRepository _rolesRepository = rolesRepository;

        public async Task<ErrorOr<List<RoleDto>>> Handle(GetRoleQuery query, CancellationToken cancellationToken)
        {
            try
            {
                if (await _rolesRepository.GetByNameAsync(query.roleType) is not List<RoleDto> roleDto)
                {
                    return Error.Validation(description: "Role not found.");
                }

                roleDto.ForEach(rDto => rDto.AffectedId = rDto.RoleId);

                return roleDto;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
