using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Application.Users.Common.Token;
using CourseManagement.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Infrastructure.RefreshTokens.Persistence
{
    public class RefreshTokensRepository : IRefreshTokensRepository
    {
        private readonly CourseManagementDbContext _dbContext;

        public RefreshTokensRepository(CourseManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _dbContext.RefreshTokens.AddAsync(token);
        }

        public async Task<List<RefreshToken>> GetActiveByUserIdAsync(int userId)
        {
            return await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ToListAsync();
        }

        public async Task<RefreshToken?> GetByHashAsync(string tokenHash)
        {
            return await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.HashedRefreshToken == tokenHash);
        }

        public async Task<List<RefreshToken>> RevokeAllForUserAsync(int userId)
        {
            return await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ToListAsync();

            //foreach (var token in userTokens)
            //{
            //    token.RevokedAt = DateTimeOffset.UtcNow;
            //    token.RevokedReason = reason;
            //}
        }

        public void Update(RefreshToken oldToken, RefreshToken newToken)
        {
            _dbContext.Entry(oldToken).CurrentValues.SetValues(newToken);
        }
    }
}
