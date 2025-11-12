using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Roles
{
    public class Role
    {
        public int Id { get; init; }
        public string RoleType { get; init; } = string.Empty;

        public List<RolesPrivileges>? RolesPrivileges { get; set; } = [];

        public Role(
            string roleType)
        {
            RoleType = roleType;
        }

        private bool ValidRoleType()
        {
            string[] roles = [
                "Guest",
                "Admin",
                "Trainer",
                "Trainee",
                "Teacher",
                "Roamer",
                "Observer",
                "Coordinator",
                "PrivilegedTrainer"];

            for (int i = 0; i < roles.Length; i++)
            {
                if (RoleType == roles[i])
                {
                    return true;
                }
            }

            return false;
        }

        public ErrorOr<Success> CheckIfRoleIsValid()
        {
            if (!ValidRoleType())
            {
                return RoleErrors.UnknownRole;
            }

            return Result.Success;
        }

        public ErrorOr<Success> AddRolePrivileges(
            List<Privilege> addPrivileges,
            List<RolesPrivileges> addRolesPrivileges)
        {
            foreach (var addPrivilege in addPrivileges)
            {
                if (addPrivilege.CheckIfPrivilegeIsValid() != Result.Success)
                {
                    return RoleErrors.RolePrivilegeInvalid;
                }
            }

            foreach (var addRolePrivilege in addRolesPrivileges)
            {
                if (RolesPrivileges is not null &&
                    RolesPrivileges.Contains(addRolePrivilege))
                {
                    return RoleErrors.RoleAlreadyHasPrivilege;
                }

                var RolePrivilege = new RolesPrivileges(
                    roleId: addRolePrivilege.RoleId,
                    privilegeId: addRolePrivilege.PrivilegeId);

                RolesPrivileges!.Add(RolePrivilege);
            }

            return Result.Success;
        }
    }
}
