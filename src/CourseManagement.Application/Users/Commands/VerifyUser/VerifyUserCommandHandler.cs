using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;
using CourseManagement.Application.Common.Behaviors;
using Microsoft.AspNetCore.Identity;
using CourseManagement.Domain.Users;

namespace CourseManagement.Application.Users.Commands.VerifyUser
{
    public class VerifyUserCommandHandler(
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<VerifyUserCommand, ErrorOr<UserShortDto>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<UserShortDto>> Handle(VerifyUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _usersRepository.GetByExactNameAsync(command.alias);

                if (user == null)
                {
                    return Error.Validation(description: "User not found.");
                }

                var authenticatedAlias = IdentityBehavior.CheckIfAuthenticationAliasMatch(command.headers, command.alias);
                if (authenticatedAlias != Result.Success)
                {
                    return authenticatedAlias.Errors;
                }

                if (user.IsVerified)
                {
                    return Error.Validation(description: "User already verified.");
                }

                if (user.VerificationCode != command.verificationCode)
                {
                    return Error.Validation(description: "Invalid verification code.");
                }

                user.PasswordHash = new PasswordHasher<User>().HashPassword(user, command.newPassword);
                user.IsVerified = true;
                user.VerificationCode = "";

                await _unitOfWork.CommitChangesAsync();

                UserShortDto verifiedUser = new UserShortDto()
                {
                    Id = user.Id,
                    Alias = user.Alias
                };

                verifiedUser.AffectedId = user.Id;

                return verifiedUser;
            }
            catch (Exception ex)
            {
                return Error.Failure($"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
