using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;

namespace CourseManagement.Application.Users.Queries.FilterUsersByUpper1
{
    public class FilterUsersByUpper1QueryHandler(
        IUsersRepository usersRepository
        ) : IRequestHandler<FilterUsersByUpper1Query, ErrorOr<List<UserDto>>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;

        public async Task<ErrorOr<List<UserDto>>> Handle(FilterUsersByUpper1Query query, CancellationToken cancellationToken)
        {
            try
            {
                var filteredUsers = await _usersRepository.FilterByUpper1Async(query.upper1Alias);
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
