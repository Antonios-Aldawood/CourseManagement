using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;
using CourseManagement.Application.Users.Common.Token;

namespace CourseManagement.Application.Users.Commands.RefreshUserToken
{
    public class RefreshUserTokenCommandHandler(
        IConfiguration configuration,
        IUsersRepository usersRepository,
        IRefreshTokensRepository refreshTokensRepository,
        IRolesRepository rolesRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<RefreshUserTokenCommand, ErrorOr<RefreshDto>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly IRefreshTokensRepository _refreshTokensRepository = refreshTokensRepository;
        private readonly IRolesRepository _rolesRepository = rolesRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<RefreshDto>> Handle(RefreshUserTokenCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var hashed = Token.ComputeSha256Hash(command.refreshToken);

                // Find user by stored hashed refresh token
                var user = await _usersRepository.GetUserByRefreshTokenHashAsync(hashed);
                if (user == null)
                {
                    return Error.Unauthorized(description: "Invalid refresh token.");
                }

                if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
                {
                    // Token expired. Optionally, wipe stored refresh token to force re-login
                    user.RefreshToken = string.Empty;
                    user.RefreshTokenExpiryTime = DateTime.MinValue;
                    await _unitOfWork.CommitChangesAsync();

                    return Error.Unauthorized(description: "Refresh token expired.");
                }

                var refreshToken = await _refreshTokensRepository.GetByHashAsync(hashed);
                if (refreshToken == null)
                {
                    return Error.Unauthorized(description: "Invalid refresh token.");
                }

                if (refreshToken.ReplacedById != null)
                {
                    var suspiciousUserTokens = await _refreshTokensRepository.RevokeAllForUserAsync(user.Id);

                    foreach (var suspiciousUserToken in suspiciousUserTokens)
                    {
                        suspiciousUserToken.RevokedAt = DateTimeOffset.UtcNow;
                        suspiciousUserToken.RevokedReason = "Reuse Detected";
                    }

                    return Error.Unauthorized(description: "Suspicious behavior for token reuse.");
                }

                if (refreshToken.RevokedAt != null)
                {
                    var suspiciousUserTokens = await _refreshTokensRepository.RevokeAllForUserAsync(user.Id);

                    foreach (var suspiciousUserToken in suspiciousUserTokens)
                    {
                        suspiciousUserToken.RevokedAt = DateTimeOffset.UtcNow;
                        suspiciousUserToken.RevokedReason = "Reuse Detected";
                    }

                    return Error.Unauthorized(description: "Suspicious behavior for token reuse.");
                }

                if (refreshToken.ExpiresAt < DateTime.UtcNow)
                {
                    refreshToken.HashedRefreshToken = string.Empty;
                    refreshToken.ExpiresAt = DateTimeOffset.MinValue;
                    await _unitOfWork.CommitChangesAsync();

                    return Error.Unauthorized(description: "Refresh token expired.");
                }

                // Rotate: generate a new refresh token & hash
                var token = new Token();
                var (newRawRefresh, newHashed) = token.GenerateRefreshTokenAndHash();

                user.RefreshToken = newHashed;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(10);

                RefreshToken newRefreshToken = new RefreshToken(
                    userId: user.Id,
                    hashedRefreshToken: newHashed,
                    expiresAt: DateTimeOffset.UtcNow.AddDays(10));

                await _refreshTokensRepository.AddAsync(newRefreshToken);

                await _unitOfWork.CommitChangesAsync();

                refreshToken.ReplacedById = newRefreshToken.Id;
                refreshToken.ReplacedBy = newRefreshToken;
                refreshToken.RevokedAt = DateTimeOffset.UtcNow;
                refreshToken.RevokedReason = "Rotated.";

                await _unitOfWork.CommitChangesAsync();

                // Generate new jwt
                var jwtResult = token.GenerateToken(
                    _rolesRepository,
                    configuration,
                    user);

                if (jwtResult.IsError)
                {
                    return jwtResult.Errors;
                }

                return new RefreshDto
                {
                    Token = jwtResult.Value,
                    RefreshToken = newRawRefresh,
                    RefreshTokenExpiryDate = user.RefreshTokenExpiryTime,

                    AffectedId = user.Id
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
