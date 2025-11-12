using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;

namespace CourseManagement.Application.Users.Queries.GetUser
{
    public class GetUserQueryHandler(
        IUsersRepository usersRepository
        ) : IRequestHandler<GetUserQuery, ErrorOr<List<UserDto>>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;

        public async Task<ErrorOr<List<UserDto>>> Handle(GetUserQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var foundUsers = await _usersRepository.GetByNameAsync(query.alias);

                if (foundUsers is not List<UserDto> users ||
                    foundUsers.Count == 0)
                {
                    return Error.Validation(description: "User not found.");
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
