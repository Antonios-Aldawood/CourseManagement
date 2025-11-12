using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CourseManagement.Application.JobSkill.Commands.Validator;

namespace CourseManagement.Application.JobSkill.Commands.CreateJobSkill
{
    public class CreateJobSkillCommandValidator : AbstractValidator<CreateJobSkillCommand>
    {
        public CreateJobSkillCommandValidator()
        {
            JobSkillValidatorRules.ApplyJobSkillRules(
                validator: this,
                getWeight: x => x.weight);
        }
    }
}
