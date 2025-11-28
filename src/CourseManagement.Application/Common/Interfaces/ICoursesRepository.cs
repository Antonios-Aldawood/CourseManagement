using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Domain.Courses;
using CourseManagement.Application.Courses.Common.Dto;

namespace CourseManagement.Application.Common.Interfaces
{
    public interface ICoursesRepository
    {
        Task AddCourseAsync(Course course);
        void UpdateCourse(Course oldCourse, Course newCourse);
        Task DeleteCourseAsync(string subject);
        Task<bool> ExistsAsync(int courseId);
        Task<bool> SubjectExistsAsync(string subject);
        Task<Course?> GetCourseByIdAsync(int courseId);
        Task<Course?[]> GetOldCourseAndCourseByIdsAsync(int oldCourseId, int courseId);
        Task<Course?> GetCourseByExactSubjectAsync(string subject);
        //No benefit of using this next one, because the new courseId could be null, so back to two roundtrips.
        //Task<List<Course>> GetOldAndNewCourseAsync(List<int> coursesIds);
        
        //No benefit of using this next one, still two database roundtrips.
        //Task<Course?> GetCourseWithSkillsByExactSubjectAsync(string subject);
        Task<List<Course>> GetCourseBySubjectAsync(string subject);
        Task<List<Course>> GetAllCoursesAsync();
        Task<(List<Course> items, int totalCount)> GetAllPaginatedCoursesAsync(int pageNumber, int pageSize);
        Task<List<CourseSkillsDto>> GetCourseWithCourseSkillsByEligibilities(int positionId, int departmentId, int jobId);
        //Task AddCourseEligibilityRangeAsync(List<Eligibility> eligibilities);
        Task<bool> ExistsEligibilityAsync(int courseId, string key, int value);
        Task<List<EligibilityDto>> GetAllEligibilitiesForCourseBySubject(int courseId);
        Task<List<EligibilityDto>> GetAllEligibilitiesForCourseBySubject(string subject);
        Task<Course?> GetCourseWithSessionsByIdAsync(int courseId);
        Task<Course?> GetCourseWithSessionsBySubjectAsync(string subject);
        Task<List<CourseWithSessionsAndTrainersDto>> GetCourseWithSessionsAndTrainersBySubjectAsync(string subject);
        Task<List<CourseDto>> GetAllCoursesThatMatchEligibilitiesAsync(int positionValue, int departmentId, int jobId);
        Task<List<Course>> GetCoursesForUserAsync(int userId);
        Task<List<Course>> GetCoursesWithSessionsForUsersEnrolledInThisCourse(int courseId);
        Task<Course?> GetCourseWithSessionsAndSessionsMaterialsByCourseIdAsync(int courseId);
        Task<Course?> GetCourseSessionMaterials(int courseId, int sessionId);
        Task<List<Course>> GetCoursesWithSessionsAndSessionsMaterials();
        Task<List<Course>> GetAllCoursesWithSessionsAndMaterialsThatMatchEligibilitiesAsync(int positionId, int departmentId, int jobId);
        bool CourseSessionMaterialFileExists(Course course, string sessionName, string materialPath);
    }
}
