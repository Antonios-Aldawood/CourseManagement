using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;
using CourseManagement.Application.Users.Common.Token;
using CourseManagement.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace CourseManagement.Application.Users.Commands.LoginUser
{
    public class LoginUserCommandHandler(
        IConfiguration configuration,
        IRolesRepository rolesRepository,
        IUsersRepository usersRepository,
        IRefreshTokensRepository refreshTokenRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<LoginUserCommand, ErrorOr<UserLoginDto>>
    {
        private readonly IRolesRepository _rolesRepository = rolesRepository;
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly IRefreshTokensRepository _refreshTokensRepository = refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<UserLoginDto>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var loginAttempt = await _usersRepository.GetByEmailAsync(command.email);

                if (loginAttempt is null)
                {
                    return Error.Validation(description: "Incorrect email or password.");
                }

                if (loginAttempt!.Email != command.email ||
                    new PasswordHasher<User>().VerifyHashedPassword(loginAttempt!, loginAttempt!.PasswordHash, command.password) == PasswordVerificationResult.Failed)
                {
                    return Error.Validation(description: "Incorrect email or password.");
                }

                var oldRefreshToken = await _refreshTokensRepository.GetByHashAsync(loginAttempt.RefreshToken);
                if (oldRefreshToken is null)
                {
                    return Error.Unauthorized(description: "Invalid refresh token.");
                }

                Token token = new();

                var (rawRefreshToken, hashRefreshToken) = token.GenerateRefreshTokenAndHash();

                /*
                var refreshToken = token.UserRefreshToken();

                if (refreshToken.IsError)
                {
                    return refreshToken.Errors;
                }

                loginAttempt.RefreshToken = refreshToken.Value;
                */

                loginAttempt.RefreshToken = hashRefreshToken;
                loginAttempt.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(10);

                var fullUser = await _usersRepository.GetFullByEmailAsync(loginAttempt.Email);

                if (fullUser is not UserFullDto)
                {
                    return Error.Unexpected(description: "User has no role nor job.");
                }

                RefreshToken refreshToken = new RefreshToken(
                    userId: fullUser.Id,
                    hashedRefreshToken: hashRefreshToken,
                    expiresAt: DateTimeOffset.UtcNow.AddDays(10));

                await _refreshTokensRepository.AddAsync(refreshToken);

                await _unitOfWork.CommitChangesAsync();

                oldRefreshToken.ReplacedById = refreshToken.Id;
                oldRefreshToken.ReplacedBy = refreshToken;
                oldRefreshToken.RevokedAt = DateTimeOffset.UtcNow;
                oldRefreshToken.RevokedReason = "Rotated.";

                await _unitOfWork.CommitChangesAsync();

                var userToken = token.GenerateToken(
                    _rolesRepository,
                    configuration,
                    loginAttempt);

                if (userToken == "Unfound role.")
                {
                    return Error.Unexpected(description: "User has no role.");
                }

                if (userToken.IsError)
                {
                    return userToken.Errors;
                }

                UserLoginDto userResponse = new UserLoginDto
                {
                    Id = loginAttempt.Id,
                    Alias = loginAttempt.Alias,
                    Email = loginAttempt.Email,
                    PhoneNumber = loginAttempt.PhoneNumber,
                    Position = loginAttempt.Position,
                    RoleType = fullUser.RoleType,
                    City = loginAttempt.Address!.City,
                    Region = loginAttempt.Address.Region,
                    Road = loginAttempt.Address.Road,
                    JobTitle = fullUser.JobTitle,
                    JobDescription = fullUser.JobDescription,
                    DepartmentName = fullUser.DepartmentName,
                    Upper1 = loginAttempt.Upper1!.Alias,
                    Upper2 = loginAttempt.Upper2!.Alias,
                    IsVerified = loginAttempt.IsVerified,
                    Token = userToken.Value,
                    RefreshToken = rawRefreshToken,
                    RefreshTokenExpiryDate = loginAttempt.RefreshTokenExpiryTime
                };

                userResponse.AffectedId = loginAttempt.Id;

                userToken = "";
                rawRefreshToken = "";

                return userResponse;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}

/*            
            if (!loginAttempt.IsVerified)
            {
                return Error.Validation(description: "Account not verified. Please check your email for the verification code.");
            }
*/
