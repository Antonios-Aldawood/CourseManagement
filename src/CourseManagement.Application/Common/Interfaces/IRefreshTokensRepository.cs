using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Users.Common.Token;

namespace CourseManagement.Application.Common.Interfaces
{
    public interface IRefreshTokensRepository
    {
        Task<RefreshToken?> GetByHashAsync(string tokenHash);
        Task<List<RefreshToken>> GetActiveByUserIdAsync(int userId);
        Task AddAsync(RefreshToken token);
        void Update(RefreshToken oldToken, RefreshToken newToken);
        Task<List<RefreshToken>> RevokeAllForUserAsync(int userId);

    }
}
