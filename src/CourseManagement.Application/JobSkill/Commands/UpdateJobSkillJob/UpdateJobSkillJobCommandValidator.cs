using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CourseManagement.Application.JobSkill.Commands.Validator;

namespace CourseManagement.Application.JobSkill.Commands.UpdateJobSkillJob
{
    public class UpdateJobSkillJobCommandValidator : AbstractValidator<UpdateJobSkillJobCommand>
    {
        public UpdateJobSkillJobCommandValidator()
        {
            JobSkillValidatorRules.ApplyJobSkillRules(
                validator: this,
                getWeight: x => x.weight ?? 3);
        }
    }
}
