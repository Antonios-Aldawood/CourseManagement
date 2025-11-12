using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Roles
{
    public class Privilege
    {
        public int Id { get; init; }
        public string PrivilegeName { get; init; } = string.Empty;

        internal Privilege(
            string privilegeName)
        {
            PrivilegeName = privilegeName;
        }

        private bool ValidPrivilegeName()
        {
            string[] privileges = [
                "AddUser",
                "GetUser",
                "ListUsers",
                "AddAddress",
                "GetAddress",
                "ListAddresses",
                "AddJob",
                "GetJob",
                "ListJobs",
                "AddDepartment",
                "GetDepartment",
                "ListDepartments",
                "AddRole",
                "GetRole",
                "ListRoles",
                "AddPrivilege",
                "GetPrivilege",
                "ListPrivileges"];

            for (int i = 0; i < privileges.Length; i++)
            {
                if (PrivilegeName == privileges[i])
                {
                    return true;
                }
            }

            return false;
        }

        public ErrorOr<Success> CheckIfPrivilegeIsValid()
        {
            if (!ValidPrivilegeName())
            {
                return PrivilegeErrors.UnknownPrivilege;
            }

            return Result.Success;
        }
    }
}
