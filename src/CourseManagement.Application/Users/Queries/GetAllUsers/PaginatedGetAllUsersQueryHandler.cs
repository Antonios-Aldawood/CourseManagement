using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Common.Dto;
using CourseManagement.Application.Users.Common.Dto;

namespace CourseManagement.Application.Users.Queries.GetAllUsers
{
    public class PaginatedGetAllUsersQueryHandler(
        IUsersRepository usersRepository
        ) : IRequestHandler<PaginatedGetAllUsersQuery, ErrorOr<PagedResult<UserDto>>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;

        public async Task<ErrorOr<PagedResult<UserDto>>> Handle(PaginatedGetAllUsersQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var (users, totalCount) = await _usersRepository
                    .GetAllUsersPaginatedAsync(query.pageNumber, query.pageSize);

                users.ForEach(user => user.AffectedId = user.UserId);

                return new PagedResult<UserDto>
                {
                    Items = users,
                    PageNumber = query.pageNumber,
                    PageSize = query.pageSize,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
