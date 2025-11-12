using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Domain.JobsSkills;
using CourseManagement.Application.JobSkill.Common.Dto;

namespace CourseManagement.Application.Common.Interfaces
{
    public interface IJobsSkillsRepository
    {
        Task AddJobsSkillsAsync(JobsSkills jobSkill);
        void UpdateJobsSkills(JobsSkills oldJobSkill, JobsSkills newJobSkill);
        Task DeleteJobsSkillsAsync(int jobSkillId);
        Task<bool> JobSkillExistsAsync(int jobSkillId);
        Task<bool> JobSkillExistsByJobAndSkillIdsAsync(int jobId, int skillId);
        Task<int> GetOldJobAndJobSkillsCountAsync(int oldJobId, int jobId);
        Task<List<JobAndJobSkillsFullDto>> GetOldJobAndJobWithJobSkillsByIdAsync(int oldJobId, int jobId);
        Task<List<SkillAndSkillJobsFullDto>> GetOldSkillAndSkillWithSkillJobsByIdAsync(int oldSkillId, int skillId);
        Task<JobsSkills?> GetJobSkillByIdAsync(int jobSkillId);
        Task<JobsSkills?> GetJobSkillByJobAndSkillIdAsync(int jobId, int skillId);
        Task<List<JobsSkills>> GetJobsSkillsByJobIdAsync(int jobId);
        Task<List<JobsSkillsDto>> GetJobsSkillsWithNamesByJobIdAsync(int jobId);
        Task<List<JobsSkills>> GetJobsSkillsByJobsIdsAsync(List<int> jobsIds);
        Task<List<JobsSkills>> GetJobsSkillsBySkillIdAsync(int skillId);
        Task<List<JobsSkillsDto>> GetJobsSkillsWithTitlesBySkillIdAsync(int skillId);
        Task<List<JobsSkills>> GetJobsSkillsBySkillsIdsAsync(List<int> skillsIds);
        Task<List<JobsSkills>> GetAllJobsSkillsAsync();
        Task<List<JobsSkillsDto>> GetAllJobsSkillsWithTitlesAndNamesAsync();
    }
}
