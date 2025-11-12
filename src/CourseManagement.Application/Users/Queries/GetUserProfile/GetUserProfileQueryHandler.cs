using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Domain.Users;

namespace CourseManagement.Application.Users.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler(
        IUsersRepository usersRepository
        ) : IRequestHandler<GetUserProfileQuery, ErrorOr<User>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;

        public async Task<ErrorOr<User>> Handle(GetUserProfileQuery query, CancellationToken cancellationToken)
        {
            try
            {
                if (await _usersRepository.GetByExactNameAsync(query.alias) is not User user)
                {
                    return Error.Validation(description: "User not found.");
                }

                return user;
            }
            catch (Exception ex)
            {
                return Error.Failure($"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
