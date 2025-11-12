using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CourseManagement.Application.Skills.Commands.Validator;

namespace CourseManagement.Application.Skills.Commands.UpdateSkill
{
    public class UpdateSkillCommandValidator : AbstractValidator<UpdateSkillCommand>
    {
        public UpdateSkillCommandValidator()
        {
            SkillValidatorRules.ApplySkillRules(
                validator: this,
                getSkillName: x => x.skillName ?? "Valid skill",
                getLevelCap: x => x.levelCap ?? 5);
        }
    }
}
