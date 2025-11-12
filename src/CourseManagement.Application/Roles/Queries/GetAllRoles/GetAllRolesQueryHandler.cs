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

namespace CourseManagement.Application.Roles.Queries.GetAllRoles
{
    public class GetAllRolesQueryHandler(
        IRolesRepository rolesRepository
        ) : IRequestHandler<GetAllRolesQuery, ErrorOr<List<RoleDto>>>
    {
        private readonly IRolesRepository _rolesRepository = rolesRepository;

        public async Task<ErrorOr<List<RoleDto>>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<Role> roles = await _rolesRepository.GetAllRolesAsync();
                List<RoleDto> roleResponse = [];
                foreach (Role role in roles)
                {
                    RoleDto dto = RoleDto.AddDto(role);

                    roleResponse.Add(dto);
                }

                return roleResponse;
            }
            catch (Exception ex)
            {
                return Error.Failure($"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
