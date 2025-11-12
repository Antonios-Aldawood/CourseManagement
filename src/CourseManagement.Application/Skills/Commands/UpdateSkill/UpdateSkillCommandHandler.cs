using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Skills.Common.Dto;

namespace CourseManagement.Application.Skills.Commands.UpdateSkill
{
    public class UpdateSkillCommandHandler(
        ISkillsRepository skillsRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateSkillCommand, ErrorOr<SkillDto>>
    {
        private readonly ISkillsRepository _skillsRepository = skillsRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<SkillDto>> Handle(UpdateSkillCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var oldSkill = await _skillsRepository.GetSkillByExactSkillNameAsync(command.oldSkillSkillName);

                if (oldSkill == null)
                {
                    return Error.Validation(description: "Skill does not exist.");
                }

                if (command.skillName != null &&
                    command.skillName != oldSkill.SkillName &&
                    await _skillsRepository.SkillExistsAsync(command.skillName))
                {
                    return Error.Validation(description: "Skill name already taken.");
                }

                var newSkill = oldSkill.UpdateSkill(
                    skillName: command.skillName ?? oldSkill.SkillName,
                    levelCap: command.levelCap ?? oldSkill.LevelCap);

                if (newSkill.IsError)
                {
                    return newSkill.Errors;
                }

                _skillsRepository.UpdateSkill(oldSkill, newSkill.Value);

                await _unitOfWork.CommitChangesAsync();

                return new SkillDto
                {
                    SkillId = newSkill.Value.Id,
                    SkillName = newSkill.Value.SkillName,
                    LevelCap = newSkill.Value.LevelCap,

                    AffectedId = newSkill.Value.Id
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
