using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Roles.Common.Dto;

namespace CourseManagement.Application.Roles.Queries.GetRolePrivileges
{
    public class GetRolePrivilegesQueryHandler(
        IRolesRepository rolesRepository
        ) : IRequestHandler<GetRolePrivilegesQuery, ErrorOr<List<PrivilegeDto>>>
    {
        private readonly IRolesRepository _rolesRepository = rolesRepository;

        public async Task<ErrorOr<List<PrivilegeDto>>> Handle(GetRolePrivilegesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<PrivilegeDto> privileges = await _rolesRepository.GetRolePrivilegesAsync(query.roleType);

                if (privileges is null ||
                    privileges.Count == 0)
                {
                    return Error.Validation(description: "Either role wasn't found, or has no privileges.");
                }

                privileges.ForEach(rp => rp.AffectedId = rp.PrivilegeId);

                return privileges;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
