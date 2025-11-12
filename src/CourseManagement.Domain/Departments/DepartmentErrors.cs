using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Departments
{
    public class DepartmentErrors
    {
        public static readonly Error DepartmentMembersOutOfRange = Error.Validation(
            "Department.DepartmentMembersOutOfRange",
            "This department's minimum and maximum number of members are out of bounds, please enter Min Members not less than 20 and Max Members not over 75, with Min Members being less than Max Members.");

        public static readonly Error DepartmentMinMembersOverMaxMembers = Error.Validation(
            "Department.DepartmentMinMembersOverMaxMembers",
            "This department's minimum amount of members is above its maximum, please enter Min Members not less than 20 and Max Members not over 75, with Min Members being less than Max Members.");
    }
}

/*
        public static readonly Error DepartmentAlreadyHasJob = Error.Validation(
            "Department.DepartmentAlreadyHasJob",
            "Department already has job with the same title.");

        public static readonly Error DepartmentDoesNotHaveJob = Error.Validation(
            "Department.DepartmentDoesNotHaveJob",
            "This department doesn't have this job."); 
*/