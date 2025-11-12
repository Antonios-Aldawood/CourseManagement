using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Roles
{
    public class RoleErrors
    {
        public static readonly Error UnknownRole = Error.Validation(
            "Role.UnknownRole",
            "This role isn't one of ours, please enter either 'Admin', 'Trainer', or 'Trainee', or one of our other known roles.");

        public static readonly Error RolePrivilegeInvalid = Error.Failure(
            "Privilege.RolePrivilegeInvalid",
            "This role's privilege is invalid.");

        public static readonly Error RoleAlreadyHasPrivilege = Error.Validation(
            "Role.RoleAlreadyHasPrivilege",
            "This role has already been given this privilege");
    }
}
