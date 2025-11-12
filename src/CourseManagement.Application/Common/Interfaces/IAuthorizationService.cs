using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Application.Common.Interfaces
{
    public interface IAuthorizationService
    {
        //Task<ErrorOr<int>> IsAuthorized(ClaimsPrincipal user, string privilege);
        Task<ErrorOr<int>> IsAuthorized(Dictionary<string, string> headers, string privilege);
    }
}
