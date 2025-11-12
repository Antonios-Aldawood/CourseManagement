using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CourseManagement.Application.Skills.Commands.Validator;

namespace CourseManagement.Application.Skills.Commands.CreateSkill
{
    public class CreateSkillCommandValidator : AbstractValidator<CreateSkillCommand>
    {
        public CreateSkillCommandValidator()
        {
            SkillValidatorRules.ApplySkillRules(
                validator: this,
                getSkillName: x => x.skillName,
                getLevelCap: x => x.levelCap);
        }
    }
}
