using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Infrastructure.Common.Persistence;
using CourseManagement.Domain.Courses;
using CourseManagement.Application.Courses.Common.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace CourseManagement.Infrastructure.Courses.Persistence
{
    public class CoursesRepository : ICoursesRepository
    {
        private readonly CourseManagementDbContext _dbContext;

        public CoursesRepository(CourseManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddCourseAsync(Course course)
        {
            string filePath = "courses_log.txt";
            string courseInfo = $"Subject: {course.Subject}, Description: {course.Description}{Environment.NewLine}";

            await File.AppendAllTextAsync(filePath, courseInfo);

            await _dbContext.Courses.AddAsync(course);
        }

        public void UpdateCourse(Course oldCourse, Course newCourse)
        {
            _dbContext.Courses.Entry(oldCourse).CurrentValues.SetValues(newCourse);
        }

        public async Task DeleteCourseAsync(string subject)
        {
            await _dbContext.Courses
                .Where(c => c.Subject == subject)
                .ExecuteDeleteAsync();
        }

        public async Task<bool> ExistsAsync(int courseId)
        {
            return await _dbContext.Courses.AsNoTracking().AnyAsync(c => c.Id == courseId);
        }

        public async Task<bool> SubjectExistsAsync(string subject)
        {
            return await _dbContext.Courses.AsNoTracking().AnyAsync(c => c.Subject == subject);
        }

        public async Task<Course?> GetCourseByIdAsync(int courseId)
        {
            return await _dbContext.Courses
                .Include(c => c.Eligibilities)
                .FirstOrDefaultAsync(c => c.Id == courseId);
        }

        public async Task<Course?[]> GetOldCourseAndCourseByIdsAsync(int oldCourseId, int courseId)
        {
            //return await _dbContext.Courses
            //    .Include(c => c.Eligibilities)
            //    .Where(c => c.Id == oldCourseId || c.Id == courseId)
            //    .ToArrayAsync();

            var result = new Course?[2]; // fixed size array

            var courses = await _dbContext.Courses
                .Include(c => c.Eligibilities)
                .Where(c => c.Id == oldCourseId || c.Id == courseId)
                .ToListAsync();

            foreach (var course in courses)
            {
                if (course.Id == oldCourseId)
                    result[0] = course;
                else if (course.Id == courseId)
                    result[1] = course;
            }

            return result;
        }

        public async Task<Course?> GetCourseByExactSubjectAsync(string subject)
        {
            return await _dbContext.Courses
                .Include(c => c.Eligibilities)
                .FirstOrDefaultAsync(c => c.Subject == subject);
        }

        ////Doesn't break clean architecture principals, business logic stays in core layers,
        ////while this only retrieves data. But since it retrieves data twice, based on a small logical operation,
        ////we opted to two separate queries instead, and this on its own visits the database twice anyway.
        /*
        public async Task<Course?> GetCourseWithSkillsByExactSubjectAsync(string subject)
        {
            var course = await _dbContext.Courses
                .Include(c => c.CoursesSkillsOwned)
                .Include(c => c.Eligibilities)
                .FirstOrDefaultAsync(c => c.Subject == subject);

            if (course != null)
            {
                var skillIds = course.CoursesSkillsOwned!.Select(cs => cs.SkillId).Distinct().ToList();

                var skills = await _dbContext.Skills
                    .Where(s => skillIds.Contains(s.Id))
                    .ToListAsync();

                course.SetSkills(skills);
            }

            return course;
        }
        */

        public async Task<List<Course>> GetCourseBySubjectAsync(string subject)
        {
            return await _dbContext.Courses
                .Include(c => c.Eligibilities)
                .Where(c => c.Subject.ToLower().Contains(subject.ToLower()))
                .ToListAsync();
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _dbContext.Courses
                .ToListAsync();
        }

        public async Task<(List<Course> items, int totalCount)> GetAllPaginatedCoursesAsync(int pageNumber, int pageSize)
        {
            //string filePath = "courses_log.txt";

            //string fileContents = await File.ReadAllTextAsync(filePath);
            //Console.WriteLine("File contents:");
            //Console.WriteLine(fileContents);

            var query = _dbContext.Courses.AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(d => d.Subject)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        //public async Task AddCourseEligibilityRangeAsync(List<Eligibility> eligibilities)
        //{
        //    await _dbContext.Eligibilities.AddRangeAsync(eligibilities);
        //}

        public async Task<bool> ExistsEligibilityAsync(
            int courseId,
            string key,
            int value)
        {
            return await _dbContext.Eligibilities
                .AsNoTracking()
                .AnyAsync(e =>
                    e.CourseId == courseId &&
                    e.Key == key &&
                    e.Value == value);
        }

        public async Task<List<EligibilityDto>> GetAllEligibilitiesForCourseBySubject(int courseId)
        {
            return await (from eligibility in _dbContext.Eligibilities
                          join course in _dbContext.Courses
                          on eligibility.CourseId equals course.Id
                          where eligibility.CourseId == courseId
                          select new EligibilityDto
                          {
                              EligibilityId = eligibility.Id,
                              Key = eligibility.Key,
                              Value = eligibility.Value,
                              Course = course.Subject,
                          }).ToListAsync();
        }

        public async Task<List<EligibilityDto>> GetAllEligibilitiesForCourseBySubject(string subject)
        {
            return await (from eligibility in _dbContext.Eligibilities
                         join course in _dbContext.Courses
                         on eligibility.CourseId equals course.Id
                         where course.Subject.ToLower().Contains(subject.ToLower())
                         select new EligibilityDto
                         {
                             EligibilityId = eligibility.Id,
                             Key = eligibility.Key,
                             Value = eligibility.Value,
                             Course = course.Subject,
                         }).ToListAsync();
        }

        public async Task<List<CourseSkillsDto>> GetCourseWithCourseSkillsByEligibilities(
            int positionId,
            int departmentId,
            int jobId)
        {
            return await (from course in _dbContext.Courses
                          join eligigibilities in _dbContext.Eligibilities
                          on course.Id equals eligigibilities.CourseId
                          join courseSkill in _dbContext.CoursesSkills
                          on course.Id equals courseSkill.CourseId
                          join skill in _dbContext.Skills
                          on courseSkill.SkillId equals skill.Id
                          where
                          course.IsForAll == true ||
                          (eligigibilities.Key == "Position" && eligigibilities.Value == positionId) ||
                          (eligigibilities.Key == "Department" && eligigibilities.Value == departmentId) ||
                          (eligigibilities.Key == "Job" && eligigibilities.Value == jobId)
                          select new CourseSkillsDto
                          {
                              CourseId = course.Id,
                              Subject = course.Subject,
                              Description = course.Description,
                              SkillName = skill.SkillName,
                              Weight = courseSkill.Weight,

                              AffectedId = course.Id
                          }).Distinct().ToListAsync();
        }

        public async Task<Course?> GetCourseWithSessionsByIdAsync(int courseId)
        {
            return await _dbContext.Courses
                .Include(c => c.Eligibilities)
                .Include(c => c.Sessions)
                .FirstOrDefaultAsync(c => c.Id == courseId);
        }

        public async Task<Course?> GetCourseWithSessionsBySubjectAsync(string subject)
        {
            return await _dbContext.Courses
                .Include(c => c.Sessions)
                .FirstOrDefaultAsync(c => c.Subject == subject);
        }

        public async Task<List<CourseWithSessionsAndTrainersDto>> GetCourseWithSessionsAndTrainersBySubjectAsync(string subject)
        {
            return await (from course in _dbContext.Courses
                          where course.Subject == subject
                          join session in _dbContext.Sessions
                          on course.Id equals session.CourseId
                          join trainer in _dbContext.Users
                          on session.TrainerId equals trainer.Id
                          select new CourseWithSessionsAndTrainersDto
                          {
                              CourseSubject = course.Subject,
                              CourseSession = session,
                              TrainerName = trainer.Alias,

                              AffectedId = session.Id
                          }).ToListAsync();
        }

        public async Task<List<CourseDto>> GetAllCoursesThatMatchEligibilitiesAsync(int positionId, int departmentId, int jobId)
        {
            return await (from eligibility in _dbContext.Eligibilities
                          join course in _dbContext.Courses on eligibility.CourseId equals course.Id
                          where
                          course.IsForAll == true ||
                          (eligibility.Key == "Position" && eligibility.Value == positionId) ||
                          (eligibility.Key == "Department" && eligibility.Value == departmentId) ||
                          (eligibility.Key == "Job" && eligibility.Value == jobId)
                          select CourseDto.AddCourseDto(course)
                          ).Distinct().ToListAsync();
        }

        public async Task<List<Course>> GetAllCoursesWithSessionsAndMaterialsThatMatchEligibilitiesAsync(int positionId, int departmentId, int jobId)
        {
            return await (from eligibility in _dbContext.Eligibilities
                          join course in _dbContext.Courses on eligibility.CourseId equals course.Id
                          where
                          course.IsForAll == true ||
                          (eligibility.Key == "Position" && eligibility.Value == positionId) ||
                          (eligibility.Key == "Department" && eligibility.Value == departmentId) ||
                          (eligibility.Key == "Job" && eligibility.Value == jobId)
                          select course
                          )
                          .Distinct()
                          .Include(c => c.Sessions!)
                            .ThenInclude(s => s.Materials)
                          .ToListAsync();
        }

        public async Task<List<Course>> GetCoursesForUserAsync(int userId)
        {
            return await (from course in _dbContext.Courses.Include(c => c.Sessions)
                          //join session in _dbContext.Sessions on course.Id equals session.CourseId
                          join enrollment in _dbContext.Enrollments on course.Id equals enrollment.CourseId
                          join user in _dbContext.Users on enrollment.UserId equals user.Id
                          where user.Id == userId
                          select course
                          ).ToListAsync();
        }
        public async Task<List<Course>> GetCoursesWithSessionsForUsersEnrolledInThisCourse(int courseId)
        {
            /*
            var query =
                from e1 in _dbContext.Enrollments
                where e1.CourseId == courseId
                join e2 in _dbContext.Enrollments on e1.UserId equals e2.UserId
                join c in _dbContext.Courses.Include(c => c.Sessions) on e2.CourseId equals c.Id
                where c.Id != courseId
                select c;

            return await query.Distinct().ToListAsync();
            */
            return await (from e1 in _dbContext.Enrollments
                          where e1.CourseId == courseId
                          join e2 in _dbContext.Enrollments on e1.UserId equals e2.UserId
                          join c in _dbContext.Courses.Include(c => c.Sessions) on e2.CourseId equals c.Id
                          where c.Id != courseId
                          select c).Distinct().ToListAsync();
        }

        public async Task<Course?> GetCourseWithSessionsAndSessionsMaterialsByCourseIdAsync(int courseId)
        {
        ///////// Multiplies rows for ever working match in join, and doesn't populate correctly /////////
            /*
            return await (from course in _dbContext.Courses
                          where course.Id == courseId
                          join session in _dbContext.Sessions on course.Id equals session.CourseId
                          join material in _dbContext.Materials on session.Id equals material.SessionId
                          select course).FirstOrDefaultAsync();
            */

            return await _dbContext.Courses
                .Include(c => c.Sessions!)
                    .ThenInclude(s => s.Materials)
                .FirstOrDefaultAsync(c => c.Id == courseId);
        }

        public async Task<Course?> GetCourseSessionMaterials(int courseId, int sessionId)
        {
            return await _dbContext.Courses
                .Include(c => c.Sessions!)
                    .ThenInclude(s => s.Materials)
                .FirstOrDefaultAsync(c => c.Id == courseId && c.Sessions!.FirstOrDefault(s => s.Id == sessionId) is Session);
        }

        public async Task<List<Course>> GetCoursesWithSessionsAndSessionsMaterials()
        {
            return await _dbContext.Courses
                .Include(c => c.Sessions!)
                    .ThenInclude(s => s.Materials)
                .ToListAsync();
        }

        public bool CourseSessionMaterialFileExists(Course course, string sessionName, string materialPath)
        {
            string? filePath = course.Sessions?
                .FirstOrDefault(s => s.Name == sessionName)?.Materials?
                    .FirstOrDefault(m => m.Path == materialPath)?.Path;

            if (filePath != null &&
                File.Exists(filePath))
            {
                return true;
            }

            return false;
        }

        public async Task<string> SaveCourseSessionMaterialAsync(string courseSubject, string sessionName, IFormFile file)
        {
            string directory = @"D:/Job/DDDCMS/src/CourseManagement.Infrastructure/Courses/CoursesSessionsMaterials";
            string safeFileName = $"{courseSubject}_{sessionName}_{file.FileName}";
            string filePath = Path.Combine(directory, safeFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        public async Task<string> UpdateSavedCourseSessionMaterialAsync(string courseSubject, string sessionName, string oldMaterialFileName, IFormFile file)
        {
            string directory = @"D:/Job/DDDCMS/src/CourseManagement.Infrastructure/Courses/CoursesSessionsMaterials";
            string oldFilePath = Path.Combine(directory, oldMaterialFileName);

            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
            }

            string safeFileName = $"{courseSubject}_{sessionName}_{file.FileName}";
            string newFilePath = Path.Combine(directory, safeFileName);

            using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return newFilePath;
        }
    }
}

/*
            /////// ONLY RETURNS EVERY COURSE THAT HAS ENROLLMENTS EXCEPT THE ONE WE HAVE ///////
        public async Task<List<Course>> GetCoursesWithSessionsForUsersEnrolledInThisCourse(int courseId)
        {
            return await (from course in _dbContext.Courses.Include(c => c.Sessions)
                          join enrollment in _dbContext.Enrollments on course.Id equals enrollment.CourseId
                          where course.Id != courseId
                          select course).ToListAsync();
        }

                                            /////// SUB QUERY ///////
        public async Task<List<Course>> GetCoursesWithSessionsForUsersEnrolledInThisCourse(int courseId)
        {
            var userIdsInCourse = _dbContext.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => e.UserId);

            return await _dbContext.Courses
                .Include(c => c.Sessions)
                .Where(c => c.Id != courseId && 
                            _dbContext.Enrollments
                                .Any(e => e.CourseId == c.Id && userIdsInCourse.Contains(e.UserId)))
                .Distinct()
                .ToListAsync();
        }

        public bool CourseSessionMaterialFileExists(Course course, string sessionName, string materialPath)
        {
            if (course.Sessions != null &&
                course.Sessions.Count != 0)
            {
                foreach (var session in course.Sessions)
                {
                    if (session.Materials != null &&
                        session.Materials.Count != 0)
                    {
                        foreach (var material in session.Materials)
                        {
                            if (material.Path == path)
                            {
                                string filePath2 = material.Path;
                            }
                        }
                    }
                }
            }

            return true;
        }
*/