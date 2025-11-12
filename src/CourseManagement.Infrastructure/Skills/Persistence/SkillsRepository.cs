using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Infrastructure.Common.Persistence;
using CourseManagement.Domain.Skills;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Infrastructure.Skills.Persistence
{
    public class SkillsRepository : ISkillsRepository
    {
        private readonly CourseManagementDbContext _dbContext;

        public SkillsRepository(CourseManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddSkillAsync(Skill skill)
        {
            await _dbContext.Skills.AddAsync(skill);
        }

        public void UpdateSkill(Skill oldSkill, Skill newSkill)
        {
            _dbContext.Entry(oldSkill).CurrentValues.SetValues(newSkill);
        }

        public async Task DeleteSkillAsync(int skillId)
        {
            await _dbContext.Skills
                .Where(s => s.Id == skillId)
                .ExecuteDeleteAsync();
        }

        public async Task<bool> SkillExistsAsync(string skillName)
        {
            return await _dbContext.Skills.AsNoTracking().AnyAsync(s => s.SkillName == skillName);
        }

        public async Task<Skill?> GetSkillByIdAsync(int skillId)
        {
            return await _dbContext.Skills.FirstOrDefaultAsync(s => s.Id == skillId);
        }

        public async Task<Skill?> GetSkillByExactSkillNameAsync(string skillName)
        {
            return await _dbContext.Skills.FirstOrDefaultAsync(s => s.SkillName == skillName);
        }

        public async Task<List<Skill>> GetSkillBySkillNameAsync(string skillName)
        {
            return await _dbContext.Skills
                .Where(s => s.SkillName.Contains(skillName))
                .ToListAsync();
        }

        public async Task<List<Skill>> GetSkillsByIdsAsync(List<int> skillIds)
        {
            return await _dbContext.Skills
                    .Where(s => skillIds.Contains(s.Id))
                    .ToListAsync();
        }

        public async Task<List<Skill>> GetSkillsBySkillsNamesAsync(List<string> skillsNames)
        {
            return await _dbContext.Skills
                .Where(s => skillsNames.Contains(s.SkillName))
                .ToListAsync();
        }

        public async Task<List<Skill>> FilterSkillsByLevelCapAsync(int levelCap)
        {
            return await _dbContext.Skills
                .Where(s => s.LevelCap == levelCap)
                .ToListAsync();
        }

        public async Task<List<Skill>> SortSkillsByLeastLevelCapAsync()
        {
            return await _dbContext.Skills
                .OrderBy(s => s.LevelCap)
                .ToListAsync();
        }

        public async Task<List<Skill>> SortSkillsByMostLevelCapAsync()
        {
            return await _dbContext.Skills
                .OrderByDescending(s => s.LevelCap)
                .ToListAsync();
        }

        public async Task<List<Skill>> GetAllSkillsAsync()
        {
            return await _dbContext.Skills.ToListAsync();
        }

        public async Task<(List<Skill> items, int totalCount)> GetAllSkillsPaginatedAsync(int pageNumber, int pageSize)
        {
            var query = _dbContext.Skills.AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(s => s.SkillName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
