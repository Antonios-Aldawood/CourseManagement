using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Infrastructure.Common.Persistence;
using CourseManagement.Domain.CoursesSkills;
using CourseManagement.Application.CourseSkill.Common.Dto;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Infrastructure.CourseSkill.Persistence
{
    public class CoursesSkillsRepository : ICoursesSkillsRepository
    {
        private readonly CourseManagementDbContext _dbContext;

        public CoursesSkillsRepository(CourseManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddCoursesSkillsAsync(CoursesSkills courseSkill)
        {
            await _dbContext.CoursesSkills.AddAsync(courseSkill);
        }

        public void UpdateCoursesSkills(CoursesSkills oldCourseSkill, CoursesSkills newCourseSkill)
        {
            _dbContext.CoursesSkills.Entry(oldCourseSkill).CurrentValues.SetValues(newCourseSkill);
        }

        public async Task DeleteCoursesSkillsAsync(int courseSkillId)
        {
            await _dbContext.CoursesSkills
                .Where(cs => cs.Id == courseSkillId)
                .ExecuteDeleteAsync();
        }

        public async Task DeleteCoursesSkillsForCourseAsync(int courseId)
        {
            await _dbContext.CoursesSkills
                .Where(cs => cs.CourseId == courseId)
                .ExecuteDeleteAsync();
        }

        public async Task<bool> CourseSkillExistsAsync(int courseSkillId)
        {
            return await _dbContext.CoursesSkills.AsNoTracking().AnyAsync(cs => cs.Id == courseSkillId);
        }

        public async Task<bool> CourseSkillExistsByCourseAndSkillIdsAsync(int courseId, int skillId)
        {
            return await _dbContext.CoursesSkills
                .AsNoTracking()
                .AnyAsync(cs => cs.CourseId == courseId && cs.SkillId == skillId);
        }

        public async Task<int> GetOldCourseAndCourseSkillsCountAsync(int oldCourseId, int courseId)
        {
            return await _dbContext.CoursesSkills
                .Where(cs => cs.CourseId == oldCourseId || cs.CourseId == courseId)
                .CountAsync();
        }

        public async Task<List<CourseAndCourseSkillsFullDto>> GetOldCourseAndCourseWithCourseSkillsByIdAsync(int oldCourseId, int courseId)
        {
            return await (from course in _dbContext.Courses
                          where course.Id == oldCourseId || course.Id == courseId
                          join courseSkill in _dbContext.CoursesSkills
                          on course.Id equals courseSkill.CourseId into courseSkillsGroup
                          from courseSkill in courseSkillsGroup.DefaultIfEmpty()
                          group courseSkill by course into grouped
                          select new CourseAndCourseSkillsFullDto
                          {
                              CourseId = grouped.Key.Id,
                              CourseSubject = grouped.Key.Subject,
                              CourseSkills = grouped.Where(cs => cs != null).ToList(),

                              AffectedId = grouped.Key.Id
                          }).OrderBy(dto =>
                                dto.CourseId == oldCourseId ? 0 :
                                dto.CourseId == courseId ? 1 : 2)
                          .ToListAsync();

            //return result.OrderBy(dto =>
            //    dto.CourseId == oldCourseId ? 0 :
            //    dto.CourseId == courseId ? 1 : 2
            //).ToList();
        }

        public async Task<List<SkillAndSkillCoursesFullDto>> GetOldSkillAndSkillWithSkillCoursesByIdAsync(int oldSkillId, int skillId)
        {
            return await (from skill in _dbContext.Skills
                          where skill.Id == oldSkillId || skill.Id == skillId
                          join courseSkill in _dbContext.CoursesSkills
                          on skill.Id equals courseSkill.SkillId into coursesSkillGroup
                          from courseSkill in coursesSkillGroup.DefaultIfEmpty()
                          group courseSkill by skill into grouped
                          select new SkillAndSkillCoursesFullDto
                          {
                              SkillId = grouped.Key.Id,
                              SkillName = grouped.Key.SkillName,
                              CoursesSkill = grouped.Where(cs => cs != null).ToList(),

                              AffectedId = grouped.Key.Id
                          }).OrderBy(dto =>
                              dto.SkillId == oldSkillId ? 0 :
                              dto.SkillId == skillId ? 1 : 2)
                          .ToListAsync();
        }

        public async Task<CoursesSkills?> GetCourseSkillByIdAsync(int courseSkillId)
        {
            return await _dbContext.CoursesSkills.FirstOrDefaultAsync(cs => cs.Id == courseSkillId);
        }

        public async Task<CoursesSkills?> GetCourseSkillByCourseAndSkillIdAsync(int courseId, int skillId)
        {
            return await _dbContext.CoursesSkills.FirstOrDefaultAsync(cs => cs.CourseId == courseId && cs.SkillId == skillId);
        }

        public async Task<List<CoursesSkills>> GetCoursesSkillsByCourseIdAsync(int courseId)
        {
            return await _dbContext.CoursesSkills
                .Where(cs => cs.CourseId == courseId)
                .ToListAsync();
        }
        
        public async Task<List<CoursesSkillsDto>> GetCoursesSkillsWithNamesByCourseIdAsync(int courseId)
        {
            return await (from coursesSkills in _dbContext.CoursesSkills
                          join course in _dbContext.Courses
                          on coursesSkills.CourseId equals course.Id
                          join skill in _dbContext.Skills
                          on coursesSkills.SkillId equals skill.Id
                          where coursesSkills.CourseId == courseId
                          select new CoursesSkillsDto
                          {
                              CourseSkillId = coursesSkills.Id,
                              Course = course.Subject,
                              CourseId = coursesSkills.CourseId,
                              Skill = skill.SkillName,
                              SkillId = coursesSkills.SkillId,
                              Weight = coursesSkills.Weight
                          }).ToListAsync();
        }

        public async Task<List<CoursesSkills>> GetCoursesSkillsByCoursesIdsAsync(List<int> coursesIds)
        {
            return await _dbContext.CoursesSkills
                .Where(cs => coursesIds.Contains(cs.CourseId))
                .ToListAsync();
        }

        public async Task<List<CoursesSkills>> GetCoursesSkillsBySkillIdAsync(int skillId)
        {
            return await _dbContext.CoursesSkills
                .Where(cs => cs.SkillId == skillId)
                .ToListAsync();
        }

        public async Task<List<CoursesSkillsDto>> GetCoursesSkillsWithSubjectsBySkillIdAsync(int skillId)
        {

            return await (from coursesSkills in _dbContext.CoursesSkills
                          join course in _dbContext.Courses
                          on coursesSkills.CourseId equals course.Id
                          join skill in _dbContext.Skills
                          on coursesSkills.SkillId equals skill.Id
                          where coursesSkills.SkillId == skillId
                          select new CoursesSkillsDto
                          {
                              CourseSkillId = coursesSkills.Id,
                              Course = course.Subject,
                              CourseId = coursesSkills.CourseId,
                              Skill = skill.SkillName,
                              SkillId = coursesSkills.SkillId,
                              Weight = coursesSkills.Weight
                          }).ToListAsync();
        }
        public async Task<List<CoursesSkills>> GetCoursesSkillsBySkillsIdsAsync(List<int> skillsIds)
        {
            return await _dbContext.CoursesSkills
                .Where(cs => skillsIds.Contains(cs.SkillId))
                .ToListAsync();
        }

        public async Task<CoursesSkillsDto?> GetCourseAndSkillByCourseAndSkillIdsAsync(int courseId, int skillId)
        {
            return await (from courseSkill in _dbContext.CoursesSkills
                          join course in _dbContext.Courses
                          on courseSkill.CourseId equals course.Id
                          join skill in _dbContext.Skills
                          on courseSkill.SkillId equals skill.Id
                          where course.Id == courseSkill.Id
                          && course.Id == courseId
                          && skill.Id == courseSkill.Id
                          && skill.Id == skillId
                          select new CoursesSkillsDto
                          {
                              CourseSkillId = courseSkill.Id,
                              CourseId = course.Id,
                              Course = course.Subject,
                              SkillId = skill.Id,
                              Skill = skill.SkillName,
                              Weight = courseSkill.Weight,
                          }).FirstOrDefaultAsync();
        }

        public async Task<CoursesSkillsWithCourseAndSkillDto?> GetFullCourseAndSkillByCourseAndSkillIdsAsync(
            int courseSkillId,
            int courseId,
            int skillId)
        {
            return await (from courseSkill in _dbContext.CoursesSkills
                          join course in _dbContext.Courses
                          on courseSkill.CourseId equals course.Id
                          join skill in _dbContext.Skills
                          on courseSkill.SkillId equals skill.Id
                          where courseSkill.Id == courseSkillId
                          && course.Id == courseSkill.CourseId
                          && course.Id == courseId
                          && skill.Id == courseSkill.SkillId
                          && skill.Id == skillId
                          select new CoursesSkillsWithCourseAndSkillDto
                          {
                              CourseSkillId = courseSkill.Id,
                              Course = course,
                              Skill = skill,
                              Weight = courseSkill.Weight
                          }).FirstOrDefaultAsync();
        }

        public async Task<CourseSkillAndIdsToUpdateDto?> GetCourseSkillAndNewUpdatingIdsAsync(
            int courseSkillId,
            int courseId,
            int skillId)
        {
            return await (from courseSkill in _dbContext.CoursesSkills
                          join course in _dbContext.Courses
                          on courseSkill.CourseId equals course.Id
                          join skill in _dbContext.Skills
                          on courseSkill.SkillId equals skill.Id
                          where courseSkill.Id == courseSkillId || course.Id == courseId || skill.Id == skillId
                          select new CourseSkillAndIdsToUpdateDto
                          {
                              CourseSkill = courseSkill,
                              CourseId = course.Id,
                              SkillId = skill.Id,

                              AffectedId = courseSkill.Id
                          }).FirstOrDefaultAsync();
        }

        public async Task<List<CoursesSkills>> GetAllCoursesSkillsAsync()
        {
            return await _dbContext.CoursesSkills.ToListAsync();
        }

        public async Task<List<CoursesSkillsDto>> GetAllCoursesSkillsWithSubjectsAndNamesAsync()
        {
            return await (from coursesSkills in _dbContext.CoursesSkills
                          join course in _dbContext.Courses
                          on coursesSkills.CourseId equals course.Id
                          join skill in _dbContext.Skills
                          on coursesSkills.SkillId equals skill.Id
                          select new CoursesSkillsDto
                          {
                              CourseSkillId = coursesSkills.Id,
                              CourseId = coursesSkills.CourseId,
                              Course = course.Subject,
                              SkillId = coursesSkills.SkillId,
                              Skill = skill.SkillName,
                              Weight = coursesSkills.Weight
                          }).ToListAsync();
        }
    }
}
