using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Infrastructure.Common.Persistence;
using CourseManagement.Domain.Enrollments;
using CourseManagement.Application.Enrollments.Common.Dto;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Infrastructure.Enrollments.Persistence
{
    internal class EnrollmentsRepository : IEnrollmentsRepository
    {
        private readonly CourseManagementDbContext _dbContext;

        public EnrollmentsRepository(CourseManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddEnrollmentAsync(Enrollment enrollment)
        {
            await _dbContext.Enrollments.AddAsync(enrollment);
        }

        public async Task<bool> ExistsAsync(int enrollmentId)
        {
            return await _dbContext.Enrollments.AsNoTracking().AnyAsync(e => e.Id == enrollmentId);
        }

        public async Task<bool> ExistsForUserCourseAsync(int userId, int courseId)
        {
            return await _dbContext.Enrollments
                .AsNoTracking()
                .AnyAsync(e => e.CourseId == courseId && e.UserId == userId);
        }

        public async Task<Enrollment?> GetByIdAsync(int enrollmentId)
        {
            return await _dbContext.Enrollments.FirstOrDefaultAsync(e => e.Id == enrollmentId);
        }
        public async Task<EnrollmentWithUserAndCourseDto?> GetEnrollmentWithUserAndCourseNamesByIdAsync(int enrollmentId)
        {
            return await (from enrollment in _dbContext.Enrollments
                          where enrollment.Id == enrollmentId
                          join user in _dbContext.Users on enrollment.UserId equals user.Id
                          join course in _dbContext.Courses on enrollment.CourseId equals course.Id
                          select new EnrollmentWithUserAndCourseDto
                          {
                              Enrollment = enrollment,
                              UserAlias = user.Alias,
                              CourseSubject = course.Subject,

                              AffectedId = enrollment.Id
                          }).FirstOrDefaultAsync();
        }

        public async Task<List<EnrollmentDto>> GetEnrollmentsForUser(int userId)
        {
            return await (from enrollment in _dbContext.Enrollments
                          join user in _dbContext.Users on enrollment.UserId equals user.Id
                          join course in _dbContext.Courses on enrollment.CourseId equals course.Id
                          where enrollment.UserId == userId
                          select new EnrollmentDto
                          {
                              EnrollmentId = enrollment.Id,
                              UserId = enrollment.UserId,
                              UserAlias = user.Alias,
                              CourseId = enrollment.CourseId,
                              CourseSubject = course.Subject,
                              IsOptional = enrollment.IsOptional,
                              IsCompleted = enrollment.IsCompleted,
                              IsConfirmed = enrollment.IsConfirmed,

                              AffectedId = enrollment.Id
                          }).ToListAsync();
        }

        public async Task<List<EnrollmentDto>> GetEnrollmentsForCourse(int courseId)
        {
            return await (from enrollment in _dbContext.Enrollments
                          join course in _dbContext.Courses on enrollment.CourseId equals course.Id
                          join user in _dbContext.Users on enrollment.UserId equals user.Id
                          where enrollment.CourseId == courseId
                          select new EnrollmentDto
                          {
                              EnrollmentId = enrollment.Id,
                              UserId = enrollment.UserId,
                              UserAlias = user.Alias,
                              CourseId = enrollment.CourseId,
                              CourseSubject = course.Subject,
                              IsOptional = enrollment.IsOptional,
                              IsCompleted = enrollment.IsCompleted,
                              IsConfirmed = enrollment.IsConfirmed,

                              AffectedId = enrollment.Id
                          }).ToListAsync();
        }

        public async Task<List<EnrollmentDto>> GetAllEnrollmentsToBeConfirmed()
        {
            return await (from enrollment in _dbContext.Enrollments
                          where enrollment.IsConfirmed == false
                          join user in _dbContext.Users on enrollment.UserId equals user.Id
                          join course in _dbContext.Courses on enrollment.CourseId equals course.Id
                          select new EnrollmentDto
                          {
                              EnrollmentId = enrollment.Id,
                              UserId = enrollment.UserId,
                              UserAlias = user.Alias,
                              CourseId = enrollment.CourseId,
                              CourseSubject = course.Subject,
                              IsOptional = enrollment.IsOptional,
                              IsCompleted = enrollment.IsCompleted,
                              IsConfirmed = enrollment.IsConfirmed,

                              AffectedId = enrollment.Id

                          }).ToListAsync();
        }

        public async Task<List<Enrollment>> GetAllAsync()
        {
            return await _dbContext.Enrollments.ToListAsync();
        }

        public async Task<int> GetEnrollmentsCountForCourse(int courseId)
        {
            return await _dbContext.Enrollments
                .AsNoTracking()
                .Where(e => e.CourseId == courseId)
                .CountAsync();
        }

        public async Task<List<EnrollmentWithCourseSessionsDto>> GetEnrollmentWithUserAndCourseInfo(int enrollmentId)
        {
            return await (from enrollment in _dbContext.Enrollments.Include(e => e.Attendances)
                          where enrollment.Id == enrollmentId
                          join course in _dbContext.Courses on enrollment.CourseId equals course.Id
                          join session in _dbContext.Sessions on course.Id equals session.CourseId
                          join trainer in _dbContext.Users on session.TrainerId equals trainer.Id
                          join user in _dbContext.Users on enrollment.UserId equals user.Id
                          select new EnrollmentWithCourseSessionsDto
                          {
                              Enrollment = enrollment,
                              CourseId = course.Id,
                              CourseSubject = course.Subject,
                              SessionId = session.Id,
                              SessionName = session.Name,
                              StartDate = session.StartDate,
                              EndDate = session.EndDate,
                              SessionTrainerId = session.TrainerId,
                              SessionTrainerAlias = trainer.Alias,
                              UserId = user.Id,
                              UserAlias = user.Alias,

                              AffectedId = enrollment.Id
                          })
                          .ToListAsync();
        }
    }
}
