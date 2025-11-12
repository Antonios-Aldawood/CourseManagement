using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Users
{
    public record UserProfileResponse(
        int Id,
        string Alias,
        string Email,
        string PhoneNumber,
        string Position,
        string Upper1Alias,
        string Upper2Alias);
}
