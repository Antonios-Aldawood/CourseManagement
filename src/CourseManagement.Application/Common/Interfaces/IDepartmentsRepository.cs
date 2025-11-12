using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Domain.Departments;

namespace CourseManagement.Application.Common.Interfaces
{
    public interface IDepartmentsRepository
    {
        Task AddDepartmentAsync(Department department);
        void UpdateDepartment(Department oldDepartment, Department newDepartment);
        Task<bool> ExistsAsync(int departmentId);
        Task<bool> NameExistsAsync(string name);
        //Task<int> DepartmentAndJobExistsAsync(string name, string title);
        Task<Department?> GetByIdAsync(int departmentId);
        Task<Department?> GetByExactNameAsync(string name);
        Task<List<Department>> GetByNameAsync(string name);
        Task<List<Department>> FilterByMembersAsync(int members);
        Task<List<Department>> SortByLeastMembersAsync();
        Task<List<Department>> SortByMostMembersAsync();
        Task<List<Department>> GetAllDepartmentsForEligibilityAsync(List<int> departmentIds);
        Task<List<Department>> GetDepartmentsAsync(List<string> names);
        Task<List<Department>> GetAllDepartmentsAsync();
        Task<(List<Department> items, int totalCount)> GetAllPaginatedDepartmentsAsync(int pageNumber, int pageSize);
        Task<Dictionary<int, string>> GetDepartmentsByIdsAsync(List<int> departmentIds);
    }
}
