using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Users
{
    public record UserResponse(
        int Id,
        string Alias,
        string Email,
        string PhoneNumber,
        string Position,
        string City,
        string Region,
        string Road,
        string Role,
        string JobTitle,
        string JobDescription,
        string DepartmentName,
        string Upper1Alias,
        string Upper2Alias,
        bool IsVerified);
}
