using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Domain.CoursesSkills;
using CourseManagement.Application.CourseSkill.Common.Dto;

namespace CourseManagement.Application.Common.Interfaces
{
    public interface ICoursesSkillsRepository
    {
        Task AddCoursesSkillsAsync(CoursesSkills courseSkill);
        void UpdateCoursesSkills(CoursesSkills oldCourseSkill, CoursesSkills newCourseSkill);
        Task DeleteCoursesSkillsAsync(int courseSkillId);
        Task DeleteCoursesSkillsForCourseAsync(int courseId);
        Task<bool> CourseSkillExistsAsync(int courseSkillId);
        Task<bool> CourseSkillExistsByCourseAndSkillIdsAsync(int courseId, int skillId);
        Task<int> GetOldCourseAndCourseSkillsCountAsync(int oldCourseId, int courseId);
        Task<List<CourseAndCourseSkillsFullDto>> GetOldCourseAndCourseWithCourseSkillsByIdAsync(int oldCourseId, int courseId);
        Task<List<SkillAndSkillCoursesFullDto>> GetOldSkillAndSkillWithSkillCoursesByIdAsync(int oldSkillId, int skillId);
        Task<CoursesSkills?> GetCourseSkillByIdAsync(int courseSkillId);
        Task<CoursesSkills?> GetCourseSkillByCourseAndSkillIdAsync(int courseId, int skillId);
        Task<List<CoursesSkills>> GetCoursesSkillsByCourseIdAsync(int courseId);
        Task<List<CoursesSkillsDto>> GetCoursesSkillsWithNamesByCourseIdAsync(int courseId);
        Task<List<CoursesSkills>> GetCoursesSkillsByCoursesIdsAsync(List<int> coursesIds);
        Task<List<CoursesSkills>> GetCoursesSkillsBySkillIdAsync(int skillId);
        Task<List<CoursesSkillsDto>> GetCoursesSkillsWithSubjectsBySkillIdAsync(int skillId);
        Task<List<CoursesSkills>> GetCoursesSkillsBySkillsIdsAsync(List<int> skillsIds);
        Task<CoursesSkillsDto?> GetCourseAndSkillByCourseAndSkillIdsAsync(int courseId, int skillId);
        Task<CoursesSkillsWithCourseAndSkillDto?> GetFullCourseAndSkillByCourseAndSkillIdsAsync(int courseSkillId, int courseId, int skillId);
        Task<CourseSkillAndIdsToUpdateDto?> GetCourseSkillAndNewUpdatingIdsAsync(int courseSkillId, int courseId, int skillId);
        Task<List<CoursesSkills>> GetAllCoursesSkillsAsync();
        Task<List<CoursesSkillsDto>> GetAllCoursesSkillsWithSubjectsAndNamesAsync();
    }
}
