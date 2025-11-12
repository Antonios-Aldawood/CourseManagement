using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Users.Common.Dto;
using CourseManagement.Domain.Courses;
using CourseManagement.Domain.Services;

namespace CourseManagement.Application.Enrollments.Commands.Validator
{
    internal static class EnrollmentEligibilityValidation
    {
        internal static bool EnrollmentEligibilityValidator(UserPosJobDepartDto user, Course course)
        {
            if (course.Eligibilities is not null && course.IsForAll == false)
            {
                int positionId = UserPositionCourseEligibilityService.ValidEligibilityPosition(user.Position);

                bool positionMatch = course.Eligibilities.Any(e => e.Key == "Position" && e.Value == positionId);
                bool departmentMatch = course.Eligibilities.Any(e => e.Key == "Department" && e.Value == user.DepartmentId);
                bool jobMatch = course.Eligibilities.Any(e => e.Key == "Job" && e.Value == user.JobId);

                if (positionMatch == false &&
                    departmentMatch == false &&
                    jobMatch == false)
                {
                    return false;
                }

                return true;
            }

            return true;
        }
    }
}
