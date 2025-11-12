using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CourseManagement.Application.JobSkill.Commands.Validator;

namespace CourseManagement.Application.JobSkill.Commands.UpdateJobSkill
{
    public class UpdateJobSkillCommandValidator : AbstractValidator<UpdateJobSkillCommand>
    {
        public UpdateJobSkillCommandValidator()
        {
            JobSkillValidatorRules.ApplyJobSkillRules(
                validator: this,
                getWeight: x => x.weight ?? 3);
        }
    }
}
