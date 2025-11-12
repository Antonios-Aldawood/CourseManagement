using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Infrastructure.Common.Persistence;
using CourseManagement.Domain.Departments;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Infrastructure.Departments.Persistence
{
    public class DepartmentsRepository : IDepartmentsRepository
    {
        private readonly CourseManagementDbContext _dbContext;

        public DepartmentsRepository(CourseManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddDepartmentAsync(Department department)
        {
            await _dbContext.Departments.AddAsync(department);
        }

        public void UpdateDepartment(Department oldDepartment, Department newDepartment)
        {
            _dbContext.Entry(oldDepartment).CurrentValues.SetValues(newDepartment);
            //_dbContext.Departments.Attach(newDepartment);
            //_dbContext.Entry(oldDepartment).Property(d => d.Id).IsModified = false;
            //_dbContext.Entry(oldDepartment).State = EntityState.Modified;
        }

        public async Task<bool> ExistsAsync(int departmentId)
        {
            return await _dbContext.Departments.AsNoTracking().AnyAsync(d => d.Id == departmentId);
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            return await _dbContext.Departments.AsNoTracking().AnyAsync(d => d.Name == name);
        }

        //public async Task<int> DepartmentAndJobExistsAsync(string? name, string? title)
        //{
        //    var result = await _dbContext.Departments
        //        .AsNoTracking()
        //        .Where(d => d.Name == name)
        //        .Select(d => new { DepartmentExists = true })
        //        .DefaultIfEmpty(new { DepartmentExists = false })
        //        .SelectMany(_ => _dbContext.Jobs
        //            .AsNoTracking()
        //            .Where(j => j.Title == title)
        //            .Select(j => new { JobExists = true })
        //            .DefaultIfEmpty(new { JobExists = false }),
        //                (department, job) => new { department.DepartmentExists, job.JobExists })
        //        .FirstOrDefaultAsync();

        //    if (result?.DepartmentExists == false)
        //    {
        //        return 1;
        //    }

        //    else if (result?.JobExists == true)
        //    {
        //        return 2;
        //    }

        //    else if (result?.DepartmentExists == true &&
        //            result?.JobExists == true)
        //    {
        //        return 3;
        //    }

        //    else
        //    {
        //        return 0;
        //    }
        //}

        public async Task<Department?> GetByIdAsync(int departmentId)
        {
            return await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == departmentId);
        }

        public async Task<Department?> GetByExactNameAsync(string name)
        {
            return await _dbContext.Departments.FirstOrDefaultAsync(d => d.Name == name);
        }

        public async Task<List<Department>> GetByNameAsync(string name)
        {
            return await _dbContext.Departments
                .Where(d => d.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();
        }

        public async Task<List<Department>> FilterByMembersAsync(int members)
        {
            return await _dbContext.Departments
                .Where(d => d.MinMembers <= members &&
                            d.MaxMembers >= members)
                .ToListAsync();
        }

        public async Task<List<Department>> SortByLeastMembersAsync()
        {
            return await _dbContext.Departments
                .OrderBy(d => d.MinMembers)
                .OrderBy(d => d.MaxMembers)
                .ToListAsync();
        }

        public async Task<List<Department>> SortByMostMembersAsync()
        {
            return await _dbContext.Departments
                .OrderByDescending(d => d.MaxMembers)
                .OrderByDescending(d => d.MinMembers)
                .ToListAsync();
        }

        public async Task<List<Department>> GetAllDepartmentsForEligibilityAsync(List<int> departmentIds)
        {
            return await _dbContext.Departments
                .Where(d => departmentIds.Contains(d.Id))
                .ToListAsync();
        }

        public async Task<List<Department>> GetDepartmentsAsync(List<string> names)
        {
            return await _dbContext.Departments
                .Where(d => names.Contains(d.Name))
                .ToListAsync();
        }

        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await _dbContext.Departments
                .ToListAsync();
        }

        public async Task<(List<Department> items, int totalCount)> GetAllPaginatedDepartmentsAsync(int pageNumber, int pageSize)
        {
            var query = _dbContext.Departments.AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(d => d.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Dictionary<int, string>> GetDepartmentsByIdsAsync(List<int> departmentIds)
        {
            return await _dbContext.Departments
                .Where(d => departmentIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id, d => d.Name);
        }
    }
}
