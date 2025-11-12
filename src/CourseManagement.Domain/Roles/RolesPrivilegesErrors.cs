using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Roles
{
    public class RolesPrivilegesErrors
    {
        public static readonly Error RoleNotFound = Error.NotFound(
            "Role.RoleNotFound",
            "Role not found.");

        public static readonly Error RoleInvalid = Error.Failure(
            "Role.RoleInvalid",
            "Role invalid.");

        public static readonly Error PrivilegeNotFound = Error.NotFound(
            "Privilege.PrivilegeNotFound",
            "Privilege not found.");

        public static readonly Error PrivilegeInvalid = Error.Failure(
            "Privilege.PrivilegeInvalid",
            "Privilege invalid.");

        public static readonly Error RoleAlreadyHasPrivilege = Error.Conflict(
            "RolesPrivileges.RoleAlreadyHasPrivilege",
            "Role already has privilege.");
    }
}
