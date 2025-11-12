using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Domain.Skills;

namespace CourseManagement.Application.Common.Interfaces
{
    public interface ISkillsRepository
    {
        Task AddSkillAsync(Skill skill);
        void UpdateSkill(Skill oldSkill, Skill newSkill);
        Task DeleteSkillAsync(int skillId);
        Task<bool> SkillExistsAsync(string skillName);
        Task<Skill?> GetSkillByIdAsync(int skillId);
        Task<Skill?> GetSkillByExactSkillNameAsync(string skillName);
        Task<List<Skill>> GetSkillBySkillNameAsync(string SkillName);
        Task<List<Skill>> GetSkillsByIdsAsync(List<int> skillIds);
        Task<List<Skill>> GetSkillsBySkillsNamesAsync(List<string> skillsNames);
        Task<List<Skill>> FilterSkillsByLevelCapAsync(int levelCap);
        Task<List<Skill>> SortSkillsByLeastLevelCapAsync();
        Task<List<Skill>> SortSkillsByMostLevelCapAsync();
        Task<List<Skill>> GetAllSkillsAsync();
        Task<(List<Skill> items, int totalCount)> GetAllSkillsPaginatedAsync(int pageNumber, int pageSize);
    }
}
