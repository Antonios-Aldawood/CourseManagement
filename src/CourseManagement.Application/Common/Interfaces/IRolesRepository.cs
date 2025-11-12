using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Domain.Roles;
using CourseManagement.Application.Roles.Common.Dto;

namespace CourseManagement.Application.Common.Interfaces
{
    public interface IRolesRepository
    {
        Task AddRoleAsync(Role role);
        Task RemoveRoleAsync(int roleId);
        Task<bool> ExistsAsync(int roleId);
        Task<bool> RoleTypeExistsAsync(string roleType);
        Task<Role?> GetByIdAsync(int roleId);
        Task<List<RoleDto>> GetByNameAsync(string roleType);
        Task<Role?> GetByExactNameAsync(string roleType);
        Task<List<Role>> GetAllRolesAsync();
        Task<List<PrivilegeDto>> GetRolePrivilegesAsync(string roleType);
        Task<List<PrivilegeDto>> GetPrivilegesDtoByRoleIdAsync(int roleId);
        Task<bool> HasPrivilege(string roleType, string privilege);
        Task<bool> PrivilegeExistsAsync(int privilegeId);
        Task<List<PrivilegeDto>> GetAllPrivilegesAsync();
        Task<bool> AddRolesPrivilegesAsync(RolesPrivileges rolesPrivileges);
        Task<bool> RolePrivilegeExistsAsync(int roleId, int privilegeId);
        Task<RolesPrivileges?> GetRolePrivilegeByIdAsync(int rolesPrivilegesId);
        Task<List<RolesPrivileges>> GetAllRolesPrivilegesAsync();
        Task<List<Privilege>> GetPrivilegesByRoleId(int roleId);
    }
}
