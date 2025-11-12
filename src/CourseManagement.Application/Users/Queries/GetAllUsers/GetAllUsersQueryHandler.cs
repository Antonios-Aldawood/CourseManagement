using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;

namespace CourseManagement.Application.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler(
        IUsersRepository usersRepository
        ) : IRequestHandler<GetAllUsersQuery, ErrorOr<List<UserDto>>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;

        public async Task<ErrorOr<List<UserDto>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<UserDto> users = await _usersRepository.GetAllUsersAsync();

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
