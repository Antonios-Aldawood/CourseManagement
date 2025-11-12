using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Jobs
{
    public class Job
    {
        public int Id { get; init; }
        public string Title { get; private set; } = string.Empty;
        public double MinSalary { get; private set; } = 0;
        public double MaxSalary { get; private set; } = 0;
        public string Description { get; private set; } = string.Empty;
        public int DepartmentId { get; set; }
        public List<int>? JobSkillsIds { get; private set; } = [];

        private Job(
            string title,
            double minSalary,
            double maxSalary,
            string description,
            int departmentId)
        {
            Title = title;
            MinSalary = minSalary;
            MaxSalary = maxSalary;
            Description = description;
            DepartmentId = departmentId;
        }

        private ErrorOr<Success> CheckIfJobIsValid()
        {
            if (MinSalary < 25000 ||
                MaxSalary > 150000)
            {
                return JobErrors.JobSalaryOutOfRange;
            }

            if (MinSalary > MaxSalary)
            {
                return JobErrors.JobMinSalOverMaxSal;
            }

            return Result.Success;
        }

        private ErrorOr<Success> CheckIfSalaryIsValid(
            double? minSalary,
            double? maxSalary)
        {
            if (minSalary < 25000 ||
                maxSalary > 150000)
            {
                return JobErrors.JobSalaryOutOfRange;
            }

            if (minSalary != null &&
                maxSalary != null &&
                minSalary > maxSalary)
            {
                return JobErrors.JobMinSalOverMaxSal;
            }

            else if (minSalary != null &&
                minSalary > MaxSalary)
            {
                return JobErrors.JobMinSalOverMaxSal;
            }

            else if (maxSalary != null &&
                MinSalary > maxSalary)
            {
                return JobErrors.JobMinSalOverMaxSal;
            }

            return Result.Success;
        }

        public static ErrorOr<Job> CreateJob(
            string title,
            double minSalary,
            double maxSalary,
            string description,
            int departmentId)
        {
            var job = new Job(
                title: title,
                minSalary: minSalary,
                maxSalary: maxSalary,
                description: description,
                departmentId: departmentId);

            if (job.CheckIfJobIsValid() != Result.Success)
            {
                return job.CheckIfJobIsValid().Errors;
            }

            return job;
        }

        public ErrorOr<Job> UpdateJob(
            string title,
            double minSalary,
            double maxSalary,
            string description,
            int departmentId)
        {
            var jobValidity = CheckIfSalaryIsValid(
                minSalary: minSalary,
                maxSalary: maxSalary);

            if (jobValidity != Result.Success)
            {
                return jobValidity.Errors;
            }

            Title = title;
            MinSalary = minSalary;
            MaxSalary = maxSalary;
            Description = description;
            DepartmentId = departmentId;

            return this;
        }
    }
}
