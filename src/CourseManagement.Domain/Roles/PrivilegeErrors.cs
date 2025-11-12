using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Roles
{
    public class PrivilegeErrors
    {
        public static readonly Error UnknownPrivilege = Error.Validation(
            "Privilege.UnknownPrivilege",
            "This privilege isn't one of ours, please enter one of our known privileges.");
    }
}
