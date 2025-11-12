using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Jobs
{
    public class JobErrors
    {
        public static readonly Error JobSalaryOutOfRange = Error.Validation(
            "Job.JobSalaryOutOfRange",
            "This job's minimum and maximum amount of salary are out of bounds, please enter Min Sal not less than 25000 and Max Sal not over 150000, with Min Sal being less than Max Sal.");

        public static readonly Error JobMinSalOverMaxSal = Error.Validation(
            "Job.JobMinSalOverMaxSal",
            "This job's minimum salary is above its maximum, please enter Min Sal not less than 25000 and Max Sal not over 150000, with Min Sal being less than Max Sal.");
    }
}
