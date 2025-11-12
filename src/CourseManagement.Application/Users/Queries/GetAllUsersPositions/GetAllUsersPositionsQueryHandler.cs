using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;

namespace CourseManagement.Application.Users.Queries.GetAllUsersPositions
{
    public class GetAllUsersPositionsQueryHandler(
        IUsersRepository usersRepository
        ) : IRequestHandler<GetAllUsersPositionsQuery, ErrorOr<List<string>>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;

        public async Task<ErrorOr<List<string>>> Handle(GetAllUsersPositionsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var positions = await _usersRepository.GetAllUsersPositionAsync();

                //if (positions is null ||
                //    positions.Count == 0)
                //{
                //    return new List<string>(); // Error.Validation(description: "No positions found.");
                //}

                return positions;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
