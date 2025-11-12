using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.Users;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CourseManagement.Application.Users.Common.Token
{
    internal class Token()
    {
        internal ErrorOr<string> GenerateToken(
            IRolesRepository rolesRepository,
            IConfiguration configuration,
            User user)
        {
            try
            {
                //Has to exist, no way a user has no role in our system, but we're making a safety check.
                var role = rolesRepository.GetByIdAsync(user.RoleId).GetAwaiter().GetResult();

                if (role == null)
                {
                    return "Unfound role.";
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Alias),
                    new Claim(ClaimTypes.Role, role!.RoleType)
                };

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

                var tokenDescriptor = new JwtSecurityToken(
                    issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                    audience: configuration.GetValue<string>("AppSettings:Audience"),
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(10),
                    signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }

        internal (string RawToken, string HashedToken) GenerateRefreshTokenAndHash()
        {
            var randomNumber = new byte[64]; // 64 bytes -> 512 bits
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var raw = Convert.ToBase64String(randomNumber);

            var hashed = ComputeSha256Hash(raw);
            return (raw, hashed);
        }

        private ErrorOr<string> GenerateRefreshToken()
        {
            try
            {
                var randomNumber = new byte[32];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomNumber);

                return Convert.ToBase64String(randomNumber);
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }

        internal static string ComputeSha256Hash(string value)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(value);
            var hashedBytes = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hashedBytes);
        }

        internal ErrorOr<string> UserRefreshToken()
        {
            var refreshToken = GenerateRefreshToken();

            if (refreshToken.IsError)
            {
                return refreshToken.Errors;
            }

            return refreshToken.Value;
        }
    }
}
