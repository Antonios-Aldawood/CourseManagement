using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Roles.Common.Dto;

namespace CourseManagement.Application.Roles.Queries.GetAllPrivileges
{
    public class GetAllPrivilegesQueryHandler(
        IRolesRepository rolesRepository
        ) : IRequestHandler<GetAllPrivilegesQuery, ErrorOr<List<PrivilegeDto>>>
    {
        private readonly IRolesRepository _rolesRepository = rolesRepository;

        public async Task<ErrorOr<List<PrivilegeDto>>> Handle(GetAllPrivilegesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<PrivilegeDto> privileges = await _rolesRepository.GetAllPrivilegesAsync();

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
