using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Infrastructure.Common.Persistence;
using CourseManagement.Domain.Roles;
using CourseManagement.Application.Roles.Common.Dto;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CourseManagement.Infrastructure.Roles.Persistence
{
    public class RolesRepository : IRolesRepository
    {
        private readonly CourseManagementDbContext _dbContext;

        public RolesRepository(CourseManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddRoleAsync(Role role)
        {
            await _dbContext.Roles.AddAsync(role);
        }

        public async Task RemoveRoleAsync(int roleId)
        {
            await _dbContext.Roles.Where(r => r.Id == roleId).ExecuteDeleteAsync();
        }

        public async Task<bool> ExistsAsync(int roleId)
        {
            return await _dbContext.Roles.AsNoTracking().AnyAsync(role => role.Id == roleId);
        }

        public async Task<bool> RoleTypeExistsAsync(string roleType)
        {
            return await _dbContext.Roles
                .AsNoTracking()
                .AnyAsync(r => r.RoleType == roleType);
        }

        public async Task<Role?> GetByIdAsync(int roleId)
        {
            return await _dbContext.Roles
                .Include(r => r.RolesPrivileges)
                .FirstOrDefaultAsync(role => role.Id == roleId);
        }

        public async Task<List<RoleDto>> GetByNameAsync(string roleType)
        {
            return await _dbContext.Roles
                .Include(r => r.RolesPrivileges)
                .Where(role => role.RoleType.ToLower().Contains(roleType.ToLower()))
                .Select(r => new RoleDto { RoleId = r.Id, RoleType = r.RoleType})
                .ToListAsync();
        }

        public async Task<Role?> GetByExactNameAsync(string roleType)
        {
            return await _dbContext.Roles
                .Include(r => r.RolesPrivileges)
                .FirstOrDefaultAsync(role => role.RoleType.ToLower() == roleType.ToLower());
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _dbContext.Roles
                .Include(r => r.RolesPrivileges)
                .ToListAsync();
        }

        public async Task<List<PrivilegeDto>> GetRolePrivilegesAsync(string roleType)
        {
            return await (from role in _dbContext.Roles
                          join rp in _dbContext.RolesPrivileges
                          on role.Id equals rp.RoleId
                          join priv in _dbContext.Privileges
                          on rp.PrivilegeId equals priv.Id
                          where role.RoleType.ToLower().Contains(roleType.ToLower())
                          select new PrivilegeDto
                          {
                              PrivilegeId = priv.Id,
                              PrivilegeName = priv.PrivilegeName

                          }).ToListAsync();
        }

        public async Task<List<PrivilegeDto>> GetPrivilegesDtoByRoleIdAsync(int roleId)
        {
            return await (from role in _dbContext.Roles
                          join rp in _dbContext.RolesPrivileges
                          on role.Id equals rp.RoleId
                          join priv in _dbContext.Privileges
                          on rp.PrivilegeId equals priv.Id
                          where role.Id == roleId
                          select new PrivilegeDto
                          {
                              PrivilegeId = priv.Id,
                              PrivilegeName = priv.PrivilegeName

                          }).ToListAsync();
        }

        public async Task<bool> HasPrivilege(string roleType, string privilege)
        {
            return await _dbContext.RolesPrivileges
                .Include(rp => rp.Role)
                .Include(rp => rp.Privilege)
                .AsNoTracking()
                .AnyAsync(rp => rp.Role!.RoleType == roleType && rp.Privilege!.PrivilegeName == privilege);
        }

        public async Task<bool> PrivilegeExistsAsync(int privilegeId)
        {
            return await _dbContext.Privileges.AsNoTracking().AnyAsync(p => p.Id == privilegeId);
        }

        public async Task<List<PrivilegeDto>> GetAllPrivilegesAsync()
        {
            return await (from priv in _dbContext.Privileges
                          select new PrivilegeDto
                          {
                              PrivilegeId = priv.Id,
                              PrivilegeName = priv.PrivilegeName
                          }).ToListAsync();
        }

        public async Task<bool> AddRolesPrivilegesAsync(RolesPrivileges rolesPrivileges)
        {
            if (await _dbContext.RolesPrivileges.FirstOrDefaultAsync(rp =>
                    rp.RoleId == rolesPrivileges.RoleId &&
                    rp.PrivilegeId == rolesPrivileges.PrivilegeId)
                is not null)
            {
                return false;
            }

            await _dbContext.RolesPrivileges.AddAsync(rolesPrivileges);

            return true;
        }

        public async Task<bool> RolePrivilegeExistsAsync(int roleId, int privilegeId)
        {
            return await _dbContext.RolesPrivileges.AsNoTracking().AnyAsync(rp =>
                rp.RoleId == roleId &&
                rp.PrivilegeId == privilegeId);
        }

        public async Task<RolesPrivileges?> GetRolePrivilegeByIdAsync(int rolesPrivilegesId)
        {
            return await _dbContext.RolesPrivileges
                .Include(rp => rp.Role)
                .Include(rp => rp.Privilege)
                .FirstOrDefaultAsync(rp => rp.Id == rolesPrivilegesId);
        }

        public async Task<List<RolesPrivileges>> GetAllRolesPrivilegesAsync()
        {
            return await _dbContext.RolesPrivileges
                .Include(rp => rp.Role)
                .Include(rp => rp.Privilege)
                .ToListAsync();
        }

        public async Task<List<Privilege>> GetPrivilegesByRoleId(int roleId)
        {
            return await _dbContext.RolesPrivileges
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Privilege)
                .Select(rp => rp.Privilege!)
                .ToListAsync();
        }
    }
}
