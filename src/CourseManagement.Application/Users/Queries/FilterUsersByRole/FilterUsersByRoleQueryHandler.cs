using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;

namespace CourseManagement.Application.Users.Queries.FilterUsersByRole
{
    public class FilterUsersByRoleQueryHandler(
        IUsersRepository usersRepository
        ) : IRequestHandler<FilterUsersByRoleQuery, ErrorOr<List<UserDto>>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;

        public async Task<ErrorOr<List<UserDto>>> Handle(FilterUsersByRoleQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var filteredUsers = await _usersRepository.FilterByRoleAsync(query.role);
                if (filteredUsers is not List<UserDto> users ||
                    filteredUsers.Count == 0)
                {
                    return Error.Validation(description: "Users not found.");
                }

                users.ForEach(u => u.AffectedId = u.UserId);

                return users;
            }
            catch (Exception ex)
            {
                return Error.Failure($"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
