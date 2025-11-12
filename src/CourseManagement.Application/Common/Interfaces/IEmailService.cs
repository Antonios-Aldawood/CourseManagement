using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task<ErrorOr<Success>> SendVerificationEmailAsync(string toEmail, string alias, string password, string verificationCode);
    }
}
