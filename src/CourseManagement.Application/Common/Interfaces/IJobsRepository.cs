using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Domain.Jobs;
using CourseManagement.Application.Jobs.Common.Dto;

namespace CourseManagement.Application.Common.Interfaces
{
    public interface IJobsRepository
    {
        Task AddJobAsync(Job job);
        void UpdateJob(Job oldJob, Job newJob);
        Task DeleteJobAsync(string title);
        Task<bool> JobExistsAsync(int jobId);
        Task<bool> JobTitleExistsAsync(string title);
        Task<Job?> GetJobByIdAsync(int jobId);
        Task<JobDto?> GetJobByIdDtoAsync(int jobId);
        Task<Job?> GetJobByExactNameAsync(string title);
        Task<JobDto?> GetJobByExactNameDtoAsync(string title);
        Task<List<JobDto>> GetJobByNameAsync(string title);
        Task<List<JobDto>> UnprivilegedGetJobByNameAsync(string title);
        //Task<List<Job>> GetOldJobAndJobAsync(string oldJobTitle, string jobTitle);
        Task<List<JobDto>> FilterJobsBySalaryAsync(double salary);
        Task<List<JobDto>> SortJobsByHighestSalaryAsync();
        Task<List<JobDto>> SortJobsByLowestSalaryAsync();
        Task<List<JobDto>> GetAllJobsAsync();
        Task<(List<JobDto> items, int totalCount)> GetAllPaginatedJobsAsync(int pageNumber, int pageSize);
        Task<List<JobDto>> UnprivilegedGetAllJobsAsync();
        Task<(List<JobDto> items, int totalCount)> UnprivilegedGetAllPaginatedJobsAsync(int pageNumber, int pageSize);
        Task<List<JobDto>> GetJobsByDepartmentAsync(string name);
        Task<List<JobDto>> GetUnprivilegedJobsByDepartmentAsync(string name);
        Task<List<JobDto>> GetDepartmentJobsAsync(int departmentId);
        Task<List<JobDto>> GetDepartmentsJobsAsync(List<int> departmentIds);
        Task<List<JobDto>> UnprivilegedGetDepartmentJobsAsync(int departmentId);
        Task<List<JobDto>> UnprivilegedGetDepartmentsJobsAsync(List<int> departmentIds);
        Task<Dictionary<int, string>> GetJobsByIdsAsync(List<int> jobIds);
    }
}
