using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CourseManagement.Application.Jobs.Commands.Validator;

namespace CourseManagement.Application.Jobs.Commands.CreateJob
{
    public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
    {
        public CreateJobCommandValidator()
        {
            JobValidatorRules.ApplyJobRules(
                validator: this,
                getTitle: x => x.title,
                getMinSalary: x => x.minSalary,
                getMaxSalary: x => x.maxSalary,
                getDescription: x => x.description);
        }
    }
}
