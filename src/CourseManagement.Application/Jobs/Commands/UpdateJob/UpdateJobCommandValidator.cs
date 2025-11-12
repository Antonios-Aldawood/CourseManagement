using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CourseManagement.Application.Jobs.Commands.Validator;

namespace CourseManagement.Application.Jobs.Commands.UpdateJob
{
    public class UpdateJobCommandValidator : AbstractValidator<UpdateJobCommand>
    {
        public UpdateJobCommandValidator()
        {
            JobValidatorRules.ApplyJobRules(
                validator: this,
                getTitle: x => x.title ?? "Valid title",
                getMinSalary: x => x.minSalary ?? 75000,
                getMaxSalary: x => x.maxSalary ?? 75000,
                getDescription: x => x.description ?? "Valid description.");
        }
    }
}
