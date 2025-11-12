using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Departments
{
    public class Department
    {
        public int Id { get; init; }
        public string Name { get; private set; } = string.Empty;
        public int MinMembers { get; private set; } = 0;
        public int MaxMembers { get; private set; } = 0;
        public string Description { get; private set; } = string.Empty;
        public List<int>? JobIds { get; private set; } = [];

        private Department(
            string name,
            int minMembers,
            int maxMembers,
            string description)
        {
            Name = name;
            MinMembers = minMembers;
            MaxMembers = maxMembers;
            Description = description;
        }

        private ErrorOr<Success> CheckIfDepartmentIsValid()
        {
            if (MinMembers < 20 ||
                MaxMembers > 75)
            {
                return DepartmentErrors.DepartmentMembersOutOfRange;
            }

            if (MinMembers > MaxMembers)
            {
                return DepartmentErrors.DepartmentMinMembersOverMaxMembers;
            }

            return Result.Success;
        }

        private ErrorOr<Success> CheckIfMembersAreValid(
            int? minMembers,
            int? maxMembers)
        {
            if (minMembers < 20 ||
                maxMembers > 75)
            {
                return DepartmentErrors.DepartmentMembersOutOfRange;
            }

            if (minMembers != null &&
                maxMembers != null &&
                minMembers > maxMembers)
            {
                return DepartmentErrors.DepartmentMinMembersOverMaxMembers;
            }

            else if (minMembers != null &&
                minMembers > MaxMembers)
            {
                return DepartmentErrors.DepartmentMinMembersOverMaxMembers;
            }

            else if (maxMembers != null &&
                MinMembers > maxMembers)
            {
                return DepartmentErrors.DepartmentMinMembersOverMaxMembers;
            }

            return Result.Success;
        }

        public static ErrorOr<Department> CreateDepartment(
            string name,
            int minMembers,
            int maxMembers,
            string description)
        {
            var department = new Department(
                name: name,
                minMembers: minMembers,
                maxMembers: maxMembers,
                description: description);

            if (department.CheckIfDepartmentIsValid() != Result.Success)
            {
                return department.CheckIfDepartmentIsValid().Errors;
            }

            return department;
        }

        public ErrorOr<Department> UpdateDepartment(
            string name,
            int minMembers,
            int maxMembers,
            string description)
        {
            var departmentValidity = CheckIfMembersAreValid(
                minMembers: minMembers,
                maxMembers: maxMembers);

            if (departmentValidity != Result.Success)
            {
                return departmentValidity.Errors;
            }

            Name = name;
            MinMembers = minMembers;
            MaxMembers = maxMembers;
            Description = description;

            return this;
        }
    }
}

/*
public static ErrorOr<Department> UpdateDepartment(
    Department oldDepartment,
    string newName,
    int newMinMembers,
    int newMaxMembers,
    string newDescription)
{
    var newDepartment = new Department(
        name: newName,
        minMembers: newMinMembers,
        maxMembers: newMaxMembers,
        description: newDescription);

    if (newDepartment.CheckIfDepartmentIsValid() != Result.Success)
    {
        return newDepartment.CheckIfDepartmentIsValid().Errors;
    }

    oldDepartment = newDepartment;

    newDepartment = null;

    return oldDepartment;
}
*/

/*
        public List<Job>? Jobs { get; private set; } = [];

        public ErrorOr<Job> AddDepartmentJob(
            string title,
            double minSalary,
            double maxSalary,
            string description)
        {
            if (Jobs is not null &&
                Jobs.Count > 0)
            {
                foreach (Job existingJob in Jobs)
                {
                    if (existingJob.Title == title)
                    {
                        return DepartmentErrors.DepartmentAlreadyHasJob;
                    }
                }
            }

            var job = Job.CreateJob(
                title: title,
                minSalary: minSalary,
                maxSalary: maxSalary,
                description: description,
                departmentId: Id);

            if (job.IsError)
            {
                return job.Errors;
            }

            if (Jobs is not null)
            {
                Jobs.Add(job.Value);
            }

            return job;
        }

        public ErrorOr<Job> UpdateDepartmentJob(
            string title,
            double minSalary,
            double maxSalary,
            string description,
            int departmentId)
        {
            if (Jobs is null ||
                Jobs.Where(j => j.Title == title).FirstOrDefault() is not Job oldJob)
            {
                return DepartmentErrors.DepartmentDoesNotHaveJob;
            }
            
            var newJob = oldJob.UpdateJob(
                title: title,
                minSalary: minSalary,
                maxSalary: maxSalary,
                description: description,
                departmentId: departmentId);

            if (newJob.IsError)
            {
                return newJob.Errors;
            }

            return newJob.Value;
        }
*/