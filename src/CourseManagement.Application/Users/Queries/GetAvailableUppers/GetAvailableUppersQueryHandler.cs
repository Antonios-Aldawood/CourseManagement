using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;

namespace CourseManagement.Application.Users.Queries.GetAvailableUppers
{
    public class GetAvailableUppersQueryHandler(
        IUsersRepository usersRepository
        ) : IRequestHandler<GetAvailableUppersQuery, ErrorOr<List<string>>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;

        public async Task<ErrorOr<List<string>>> Handle(GetAvailableUppersQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var availableUppers = await _usersRepository.GetUpperAliasesByUserDepartmentAsync(query.jobTitle);

                //if (availableUppers is null ||
                //    availableUppers.Count == 0)
                //{
                //    return Error.Validation(description: "No uppers from this department, found.");
                //}

                return availableUppers;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
