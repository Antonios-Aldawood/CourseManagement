using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using ErrorOr;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace CourseManagement.Application.Common.Authorization
{
    public class AuthorizationService(
        IRolesRepository rolesRepository) : IAuthorizationService
    {
        private readonly IRolesRepository _rolesRepository = rolesRepository;

        public async Task<ErrorOr<int>> IsAuthorized(Dictionary<string, string> headers, string privilege)
        {
            try
            {
                if (!headers.TryGetValue("Authorization", out var authHeader))
                {
                    return Error.Unauthorized(description: "User unauthenticated.");
                }

                string tokenString = authHeader.Trim();

                const string bearerPrefix = "Bearer ";
                if (tokenString != null && tokenString.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    tokenString = tokenString.Substring(bearerPrefix.Length).Trim();
                }
                else
                {
                    return Error.Unexpected(description: "Invalid header.");
                }

                var handler = new JwtSecurityTokenHandler();

                if (!handler.CanReadToken(tokenString))
                {
                    return Error.Unexpected(description: "Invalid token.");
                }

                var jwtToken = handler.ReadJwtToken(tokenString);

                var roleName = jwtToken.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(roleName))
                {
                    return Error.Unexpected(description: "Invalid user, has no role.");
                }

                if (!await _rolesRepository.HasPrivilege(roleName, privilege))
                {
                    return Error.Forbidden(description: "User forbidden from access.");
                }

                var userId = jwtToken.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userId, out int Id))
                {
                    return Error.Unexpected(description: "Invalid user, has no identifier.");
                }

                return Id;
            }
            catch (SecurityTokenException ex)
            {
                return Error.Forbidden(description: $"Invalid token: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.Message}");
            }
        }
    }
}

/*
        public async Task<ErrorOr<int>> IsAuthorized(ClaimsPrincipal user, string privilege)
        {
            try
            {
                if (user?.Identity?.IsAuthenticated != true)
                {
                    return Error.Unauthorized(description: "User unauthenticated.");
                }

                var roleName = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(roleName))
                {
                    return Error.Unexpected(description: "Invalid user, has no role.");
                }

                if (!await _rolesRepository.HasPrivilege(roleName, privilege))
                {
                    return Error.Forbidden(description: "User forbidden from access.");
                }

                var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userId, out int Id))
                {
                    return Error.Unexpected(description: "Invalid user, has no identifier.");
                }

                return Id;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: ex.Message);
            }
        }
*/