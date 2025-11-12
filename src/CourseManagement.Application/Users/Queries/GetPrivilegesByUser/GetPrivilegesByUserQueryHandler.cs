using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Roles.Common.Dto;
using CourseManagement.Domain.Users;

namespace CourseManagement.Application.Users.Queries.GetPrivilegesByUser
{
    public class GetPrivilegesByUserQueryHandler(
        IUsersRepository usersRepository,
        IRolesRepository rolesRepository
        ) : IRequestHandler<GetPrivilegesByUserQuery, ErrorOr<List<PrivilegeDto>>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly IRolesRepository _rolesRepository = rolesRepository;

        public async Task<ErrorOr<List<PrivilegeDto>>> Handle(GetPrivilegesByUserQuery query, CancellationToken cancellationToken)
        {
            try
            {
                if (await _usersRepository.GetByExactNameAsync(query.alias) is not User user)
                {
                    return Error.Validation(description: "User not found.");
                }

                List<PrivilegeDto> privileges = await _rolesRepository.GetPrivilegesDtoByRoleIdAsync(user.RoleId);
                if (privileges is null ||
                    privileges.Count == 0)
                {
                    return Error.Validation(description: "User has no privileges.");
                }

                privileges.ForEach(p => p.AffectedId = p.PrivilegeId);

                return privileges;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
