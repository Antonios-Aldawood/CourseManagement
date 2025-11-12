using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CourseManagement.Application.Common.Behaviors
{
    internal class IdentityBehavior
    {
        private static JwtSecurityToken JwtDecoder(Dictionary<string, string> headers)
        {
            headers.TryGetValue("Authorization", out var authHeader);

            string tokenString = authHeader!.Trim();
            const string bearerPrefix = "Bearer ";
            tokenString = tokenString.Substring(bearerPrefix.Length).Trim();

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenString);

            return jwtToken;
        }

        internal static ErrorOr<Success> CheckIfAuthenticationIdMatch(Dictionary<string, string> headers, int id)
        {
            try
            {
                var jwtToken = JwtDecoder(headers);

                var userId = jwtToken.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                int.TryParse(userId, out int Id);

                if (id != Id)
                {
                    return Error.Unauthorized(description: "From identity behavior - Entered id doesn't match user's id.");
                }

                return Result.Success;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"From identity behavior - {ex.GetType().Name}: {ex.Message}");
            }
        }

        internal static ErrorOr<Success> CheckIfAuthenticationAliasMatch(Dictionary<string, string> headers, string alias)
        {
            try
            {
                var jwtToken = JwtDecoder(headers);

                var userAlias = jwtToken.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(userAlias))
                {
                    return Error.Unauthorized(description: "From identity behavior - User token has faulty claims, or user has no alias, which is not allowed for our users.");
                }
                
                if (userAlias != alias)
                {
                    return Error.Unauthorized(description: "From identity behavior - Entered alias doesn't match user's alias.");
                }

                return Result.Success;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"From identity behavior - {ex.GetType().Name}: {ex.Message}");
            }
        }

        internal static ErrorOr<Success> CheckIfAuthenticationRoleMatch(Dictionary<string, string> headers, string role)
        {
            try
            {
                var jwtToken = JwtDecoder(headers);

                var userRole = jwtToken.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(userRole))
                {
                    return Error.Unauthorized(description: "From identity behavior - User token has faulty claims, or user has no role, which is not allowed for our users.");
                }

                if (userRole != role)
                {
                    return Error.Forbidden(description: "From identity behavior - Entered role doesn't match user's role.");
                }

                return Result.Success;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"From identity behavior - {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
