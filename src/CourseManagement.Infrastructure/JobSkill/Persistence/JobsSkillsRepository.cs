using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Infrastructure.Common.Persistence;
using CourseManagement.Domain.JobsSkills;
using CourseManagement.Application.JobSkill.Common.Dto;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Infrastructure.JobSkill.Persistence
{
    public class JobsSkillsRepository : IJobsSkillsRepository
    {
        private readonly CourseManagementDbContext _dbContext;

        public JobsSkillsRepository(CourseManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddJobsSkillsAsync(JobsSkills jobSkill)
        {
            await _dbContext.JobsSkills.AddAsync(jobSkill);
        }

        public void UpdateJobsSkills(JobsSkills oldJobSkill, JobsSkills newJobSkill)
        {
            _dbContext.JobsSkills.Entry(oldJobSkill).CurrentValues.SetValues(newJobSkill);
        }

        public async Task DeleteJobsSkillsAsync(int jobSkillId)
        {
            await _dbContext.JobsSkills
                .Where(js => js.Id == jobSkillId)
                .ExecuteDeleteAsync();
        }

        public async Task<bool> JobSkillExistsAsync(int jobSkillId)
        {
            return await _dbContext.JobsSkills.AsNoTracking().AnyAsync(js => js.Id == jobSkillId);
        }

        public async Task<bool> JobSkillExistsByJobAndSkillIdsAsync(int jobId, int skillId)
        {
            return await _dbContext.JobsSkills
                .AsNoTracking()
                .AnyAsync(js => js.JobId == jobId && js.SkillId == skillId);
        }

        public async Task<int> GetOldJobAndJobSkillsCountAsync(int oldJobId, int jobId)
        {
            return await _dbContext.JobsSkills
                .Where(js => js.JobId == oldJobId || js.JobId == jobId)
                .CountAsync();
        }

        public async Task<List<JobAndJobSkillsFullDto>> GetOldJobAndJobWithJobSkillsByIdAsync(int oldJobId, int jobId)
        {
            return await (from job in _dbContext.Jobs
                          where job.Id == oldJobId || job.Id == jobId
                          join jobSkill in _dbContext.JobsSkills
                          on job.Id equals jobSkill.JobId into jobSkillGroup
                          from jobSkill in jobSkillGroup.DefaultIfEmpty()
                          group jobSkill by job into grouped
                          select new JobAndJobSkillsFullDto
                          {
                              JobId = grouped.Key.Id,
                              JobTitle = grouped.Key.Title,
                              JobSkills = grouped.Where(js => js != null).ToList(),

                              AffectedId = grouped.Key.Id,
                          }).OrderBy(dto =>
                                dto.JobId == oldJobId ? 0 :
                                dto.JobId == jobId ? 1 : 2)
                          .ToListAsync();
        }

        public async Task<List<SkillAndSkillJobsFullDto>> GetOldSkillAndSkillWithSkillJobsByIdAsync(int oldSkillId, int skillId)
        {
            return await (from skill in _dbContext.Skills
                          where skill.Id == oldSkillId || skill.Id == skillId
                          join jobSkill in _dbContext.JobsSkills
                          on skill.Id equals jobSkill.SkillId into jobSkillGrouped
                          from jobSkill in jobSkillGrouped.DefaultIfEmpty()
                          group jobSkill by skill into grouped
                          select new SkillAndSkillJobsFullDto
                          {
                              SkillId = grouped.Key.Id,
                              SkillName = grouped.Key.SkillName,
                              JobsSkill = grouped.Where(js => js != null).ToList(),

                              AffectedId = grouped.Key.Id
                          }).OrderBy(dto =>
                                dto.SkillId == oldSkillId ? 0 :
                                dto.SkillId == skillId ? 1 : 2)
                          .ToListAsync();
        }

        public async Task<JobsSkills?> GetJobSkillByIdAsync(int jobSkillId)
        {
            return await _dbContext.JobsSkills.FirstOrDefaultAsync(js => js.Id == jobSkillId);
        }

        public async Task<JobsSkills?> GetJobSkillByJobAndSkillIdAsync(int jobId, int skillId)
        {
            return await _dbContext.JobsSkills.FirstOrDefaultAsync(js => js.JobId == jobId && js.SkillId == skillId);
        }

        public async Task<List<JobsSkills>> GetJobsSkillsByJobIdAsync(int jobId)
        {
            return await _dbContext.JobsSkills
                .Where(js => js.JobId == jobId)
                .ToListAsync();
        }

        public async Task<List<JobsSkillsDto>> GetJobsSkillsWithNamesByJobIdAsync(int jobId)
        {
            return await (from jobsSkills in _dbContext.JobsSkills
                          join job in _dbContext.Jobs
                          on jobsSkills.JobId equals job.Id
                          join department in _dbContext.Departments
                          on job.DepartmentId equals department.Id
                          join skill in _dbContext.Skills
                          on jobsSkills.SkillId equals skill.Id
                          where jobsSkills.JobId == jobId
                          select new JobsSkillsDto
                          {
                              JobSkillId = jobsSkills.Id,
                              DepartmentId = department.Id,
                              DepartmentName = department.Name,
                              JobId = jobsSkills.JobId,
                              JobTitle = job.Title,
                              SkillId = jobsSkills.SkillId,
                              Skill = skill.SkillName,
                              Weight = jobsSkills.Weight
                          }).ToListAsync();
        }

        public async Task<List<JobsSkills>> GetJobsSkillsByJobsIdsAsync(List<int> jobsIds)
        {
            return await _dbContext.JobsSkills
                .Where(js => jobsIds.Contains(js.JobId))
                .ToListAsync();
        }

        public async Task<List<JobsSkills>> GetJobsSkillsBySkillIdAsync(int skillId)
        {
            return await _dbContext.JobsSkills
                .Where(js => js.SkillId == skillId)
                .ToListAsync();
        }

        public async Task<List<JobsSkillsDto>> GetJobsSkillsWithTitlesBySkillIdAsync(int skillId)
        {

            return await (from jobsSkills in _dbContext.JobsSkills
                          join job in _dbContext.Jobs
                          on jobsSkills.JobId equals job.Id
                          join department in _dbContext.Departments
                          on job.DepartmentId equals department.Id
                          join skill in _dbContext.Skills
                          on jobsSkills.SkillId equals skill.Id
                          where jobsSkills.SkillId == skillId
                          select new JobsSkillsDto
                          {
                              JobSkillId = jobsSkills.Id,
                              DepartmentId = department.Id,
                              DepartmentName = department.Name,
                              JobId = jobsSkills.JobId,
                              JobTitle = job.Title,
                              SkillId = jobsSkills.SkillId,
                              Skill = skill.SkillName,
                              Weight = jobsSkills.Weight
                          }).ToListAsync();
        }

        public async Task<List<JobsSkills>> GetJobsSkillsBySkillsIdsAsync(List<int> skillsIds)
        {
            return await _dbContext.JobsSkills
                .Where(js => skillsIds.Contains(js.SkillId))
                .ToListAsync();
        }

        public async Task<List<JobsSkills>> GetAllJobsSkillsAsync()
        {
            return await _dbContext.JobsSkills.ToListAsync();
        }

        public async Task<List<JobsSkillsDto>> GetAllJobsSkillsWithTitlesAndNamesAsync()
        {
            return await (from jobsSkills in _dbContext.JobsSkills
                          join job in _dbContext.Jobs
                          on jobsSkills.JobId equals job.Id
                          join department in _dbContext.Departments
                          on job.DepartmentId equals department.Id
                          join skill in _dbContext.Skills
                          on jobsSkills.SkillId equals skill.Id
                          select new JobsSkillsDto
                          {
                              JobSkillId = jobsSkills.Id,
                              DepartmentId = department.Id,
                              DepartmentName = department.Name,
                              JobId = jobsSkills.JobId,
                              JobTitle = job.Title,
                              SkillId = jobsSkills.SkillId,
                              Skill = skill.SkillName,
                              Weight = jobsSkills.Weight
                          }).ToListAsync();
        }
    }
}
