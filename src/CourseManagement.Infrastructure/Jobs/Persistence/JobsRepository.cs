using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Infrastructure.Common.Persistence;
using CourseManagement.Domain.Jobs;
using CourseManagement.Application.Jobs.Common.Dto;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Infrastructure.Jobs.Persistence
{
    public class JobsRepository : IJobsRepository
    {
        private readonly CourseManagementDbContext _dbContext;

        public JobsRepository(CourseManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddJobAsync(Job job)
        {
            await _dbContext.Jobs.AddAsync(job);
        }

        public void UpdateJob(Job oldJob, Job newJob)
        {
            _dbContext.Jobs.Entry(oldJob).CurrentValues.SetValues(newJob);
        }

        public async Task DeleteJobAsync(string title)
        {
            await _dbContext.Jobs
                .Where(j => j.Title == title)
                .ExecuteDeleteAsync();
        }

        public async Task<bool> JobExistsAsync(int jobId)
        {
            return await _dbContext.Jobs.AsNoTracking().AnyAsync(j => j.Id == jobId);
        }

        public async Task<bool> JobTitleExistsAsync(string title)
        {
            return await _dbContext.Jobs.AsNoTracking().AnyAsync(j => j.Title == title);
        }

        public async Task<Job?> GetJobByIdAsync(int jobId)
        {
            return await _dbContext.Jobs
                .FirstOrDefaultAsync(j => j.Id == jobId);
        }
        public async Task<JobDto?> GetJobByIdDtoAsync(int jobId)
        {
            return await (from job in _dbContext.Jobs
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where job.Id == jobId
                          select new JobDto
                          {
                              JobId = job.Id,
                              Title = job.Title,
                              MinSalary = job.MinSalary,
                              MaxSalary = job.MaxSalary,
                              Description = job.Description,
                              Department = dept.Name
                          }).FirstOrDefaultAsync();
        }

        public async Task<Job?> GetJobByExactNameAsync(string title)
        {
            return await _dbContext.Jobs
                .FirstOrDefaultAsync(j => j.Title == title);
        }

        public async Task<JobDto?> GetJobByExactNameDtoAsync(string title)
        {
            return await (from job in _dbContext.Jobs
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where job.Title == title
                          select new JobDto
                          {
                              JobId = job.Id,
                              Title = job.Title,
                              MinSalary = job.MinSalary,
                              MaxSalary = job.MaxSalary,
                              Description = job.Description,
                              Department = dept.Name
                          }).FirstOrDefaultAsync();
        }

        public async Task<List<JobDto>> GetJobByNameAsync(string title)
        {
            return await (from job in _dbContext.Jobs
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where job.Title.ToLower().Contains(title.ToLower())
                          select new JobDto
                          {
                              JobId = job.Id,
                              Title = job.Title,
                              MinSalary = job.MinSalary,
                              MaxSalary = job.MaxSalary,
                              Description = job.Description,
                              Department = dept.Name
                          }).ToListAsync();
        }

        public async Task<List<JobDto>> UnprivilegedGetJobByNameAsync(string title)
        {
            return await (from job in _dbContext.Jobs
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where job.Title.ToLower().Contains(title.ToLower())
                          select new JobDto
                          {
                              JobId = job.Id,
                              Title = job.Title,
                              Description = job.Description,
                              Department = dept.Name
                          }).ToListAsync();
        }

        //public async Task<List<Job>> GetOldJobAndJobAsync(string oldJobTitle, string jobTitle)
        //{
        //    return await _dbContext.Jobs
        //        .Where(j => j.Title == oldJobTitle || j.Title == jobTitle)
        //        .OrderBy(j => j.Title == oldJobTitle ? 0 : j.Title == jobTitle ? 1 : 2)
        //        .ToListAsync();
        //}

        public async Task<List<JobDto>> FilterJobsBySalaryAsync(double salary)
        {
            return await (from job in _dbContext.Jobs
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where job.MinSalary < salary && job.MaxSalary > salary
                          select new JobDto
                          {
                              JobId = job.Id,
                              Title = job.Title,
                              MinSalary = job.MinSalary,
                              MaxSalary = job.MaxSalary,
                              Description = job.Description,
                              Department = dept.Name
                          }).ToListAsync();
        }

        public async Task<List<JobDto>> SortJobsByHighestSalaryAsync()
        {
            return await (from job in _dbContext.Jobs
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          orderby job.MaxSalary descending, job.MinSalary descending
                          select new JobDto
                          {
                              JobId = job.Id,
                              Title = job.Title,
                              MinSalary = job.MinSalary,
                              MaxSalary = job.MaxSalary,
                              Description = job.Description,
                              Department = dept.Name
                          }).ToListAsync();
        }

        public async Task<List<JobDto>> SortJobsByLowestSalaryAsync()
        {
            return await (from job in _dbContext.Jobs
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          orderby job.MinSalary ascending, job.MaxSalary ascending
                          select new JobDto
                          {
                              JobId = job.Id,
                              Title = job.Title,
                              MinSalary = job.MinSalary,
                              MaxSalary = job.MaxSalary,
                              Description = job.Description,
                              Department = dept.Name
                          }).ToListAsync();
        }

        public async Task<List<JobDto>> GetAllJobsAsync()
        {
            return await (from job in _dbContext.Jobs
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          select new JobDto
                          {
                              JobId = job.Id,
                              Title = job.Title,
                              MinSalary = job.MinSalary,
                              MaxSalary = job.MaxSalary,
                              Description = job.Description,
                              Department = dept.Name
                          }).ToListAsync();
        }

        public async Task<(List<JobDto> items, int totalCount)> GetAllPaginatedJobsAsync(int pageNumber, int pageSize)
        {
            var query = _dbContext.Jobs.AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await (from job in _dbContext.Jobs
                               join dept in _dbContext.Departments
                               on job.DepartmentId equals dept.Id
                               select new JobDto
                               {
                                   JobId = job.Id,
                                   Title = job.Title,
                                   MinSalary = job.MinSalary,
                                   MaxSalary = job.MaxSalary,
                                   Description = job.Description,
                                   Department = dept.Name
                               })
                               .OrderBy(j => j.Title)
                               .Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<JobDto>> UnprivilegedGetAllJobsAsync()
        {
            return await (from job in _dbContext.Jobs
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          select new JobDto
                          {
                              JobId = job.Id,
                              Title = job.Title,
                              Description = job.Description,
                              Department = dept.Name
                          }).ToListAsync();
        }

        public async Task<(List<JobDto> items, int totalCount)> UnprivilegedGetAllPaginatedJobsAsync(int pageNumber, int pageSize)
        {
            var query = _dbContext.Jobs.AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await (from job in _dbContext.Jobs
                               join dept in _dbContext.Departments
                               on job.DepartmentId equals dept.Id
                               select new JobDto
                               {
                                   JobId = job.Id,
                                   Title = job.Title,
                                   Description = job.Description,
                                   Department = dept.Name
                               })
                               .OrderBy(j => j.Title)
                               .Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<JobDto>> GetJobsByDepartmentAsync(string name)
        {
            return await (from job in _dbContext.Jobs
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where dept.Name.ToLower().Contains(name.ToLower())
                          select new JobDto
                          {
                              JobId = job.Id,
                              Title = job.Title,
                              MinSalary = job.MinSalary,
                              MaxSalary = job.MaxSalary,
                              Description = job.Description,
                              Department = dept.Name
                          }).ToListAsync();
        }

        public async Task<List<JobDto>> GetUnprivilegedJobsByDepartmentAsync(string name)
        {
            return await (from job in _dbContext.Jobs
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where dept.Name.ToLower().Contains(name.ToLower())
                          select new JobDto
                          {
                              JobId = job.Id,
                              Title = job.Title,
                              Description = job.Description,
                              Department = dept.Name
                          }).ToListAsync();
        }

        public async Task<List<JobDto>> GetDepartmentJobsAsync(int departmentId)
        {
            return await (from job in _dbContext.Jobs
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where job.DepartmentId == departmentId
                          select new JobDto
                          {
                              JobId = job.Id,
                              Title = job.Title,
                              MinSalary = job.MinSalary,
                              MaxSalary = job.MaxSalary,
                              Description = job.Description,
                              Department = dept.Name
                          }).ToListAsync();
        }

        public async Task<List<JobDto>> GetDepartmentsJobsAsync(List<int> departmentIds)
        {
            return await (from job in _dbContext.Jobs
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where departmentIds.Contains(job.DepartmentId)
                          select new JobDto
                          {
                              JobId = job.Id,
                              Title = job.Title,
                              MinSalary = job.MinSalary,
                              MaxSalary = job.MaxSalary,
                              Description = job.Description,
                              Department = dept.Name
                          }).ToListAsync();
        }

        public async Task<List<JobDto>> UnprivilegedGetDepartmentJobsAsync(int departmentId)
        {
            return await (from job in _dbContext.Jobs
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where job.DepartmentId == departmentId
                          select new JobDto
                          {
                              JobId = job.Id,
                              Title = job.Title,
                              Description = job.Description,
                              Department = dept.Name
                          }).ToListAsync();
        }

        public async Task<List<JobDto>> UnprivilegedGetDepartmentsJobsAsync(List<int> departmentIds)
        {
            return await (from job in _dbContext.Jobs
                          join dept in _dbContext.Departments
                          on job.DepartmentId equals dept.Id
                          where departmentIds.Contains(job.DepartmentId)
                          select new JobDto
                          {
                              JobId = job.Id,
                              Title = job.Title,
                              Description = job.Description,
                              Department = dept.Name
                          }).ToListAsync();
        }

        public async Task<Dictionary<int, string>> GetJobsByIdsAsync(List<int> jobIds)
        {
            return await _dbContext.Jobs
                .Where(j => jobIds.Contains(j.Id))
                .ToDictionaryAsync(j => j.Id, j => j.Title);
        }
    }
}
