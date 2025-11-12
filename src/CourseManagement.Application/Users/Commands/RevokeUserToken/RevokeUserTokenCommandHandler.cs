using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;

namespace CourseManagement.Application.Users.Commands.RevokeUserToken
{
    public class RevokeUserTokenCommandHandler(
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<RevokeUserTokenCommand, ErrorOr<Success>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<Success>> Handle(RevokeUserTokenCommand command, CancellationToken cancellationToken)
        {
            try
            {
                bool idIsParsed = int.TryParse(command.userId, out var userId);
                if (idIsParsed == false)
                {
                    return Error.Unauthorized(description: "Invalid refresh token.");
                }

                var user = await _usersRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return Error.Unauthorized(description: "Invalid refresh token.");
                }

                user.RefreshToken = string.Empty;
                user.RefreshTokenExpiryTime = DateTime.MinValue;
                await _unitOfWork.CommitChangesAsync();

                return Result.Success;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
