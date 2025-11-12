using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Users
{
    public record RefreshResponse(
        string token,
        string refreshToken,
        DateTime refreshTokenExpiry);
}
