using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Domain.Enrollments;
using CourseManagement.Application.Enrollments.Common.Dto;

namespace CourseManagement.Application.Common.Interfaces
{
    public interface IEnrollmentsRepository
    {
        Task AddEnrollmentAsync(Enrollment enrollment);
        Task<bool> ExistsAsync(int enrollmentId);
        Task<bool> ExistsForUserCourseAsync(int userId, int courseId);
        //Can't work because there is no joining entity that certifies existence of one of the ids in it.
        //Task<bool> UserAndCourseExistAsync(int userId, int courseId);
        Task<Enrollment?> GetByIdAsync(int enrollmentId);
        Task<List<EnrollmentDto>> GetEnrollmentsForUser(int userId);
        Task<List<EnrollmentDto>> GetEnrollmentsForCourse(int courseId);
        Task<List<Enrollment>> GetAllAsync();
    }
}
