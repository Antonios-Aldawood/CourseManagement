using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;

namespace CourseManagement.Application.Users.Queries.FilterUsersByUpper2
{
    public class FilterUsersByUpper2QueryHandler(
        IUsersRepository usersRepository
        ) : IRequestHandler<FilterUsersByUpper2Query, ErrorOr<List<UserDto>>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;

        public async Task<ErrorOr<List<UserDto>>> Handle(FilterUsersByUpper2Query query, CancellationToken cancellationToken)
        {
            try
            {
                var filteredUsers = await _usersRepository.FilterByUpper2Async(query.upper2Alias);
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
