using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Users
{
    public record RefreshRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
