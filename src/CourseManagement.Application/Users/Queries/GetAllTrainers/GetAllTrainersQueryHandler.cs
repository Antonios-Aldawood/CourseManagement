using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;

namespace CourseManagement.Application.Users.Queries.GetAllTrainers
{
    public class GetAllTrainersQueryHandler(
        IUsersRepository usersRepository
        ) : IRequestHandler<GetAllTrainersQuery, ErrorOr<List<UserShortDto>>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;

        public async Task<ErrorOr<List<UserShortDto>>> Handle(GetAllTrainersQuery query, CancellationToken cancellationToken)
        {
            try
            {
                return await _usersRepository.GetAllTrainersAsync();
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
