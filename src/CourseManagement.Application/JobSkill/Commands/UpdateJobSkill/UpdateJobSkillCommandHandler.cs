using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.JobSkill.Common.Dto;
using CourseManagement.Domain.Jobs;
using CourseManagement.Domain.Skills;

namespace CourseManagement.Application.JobSkill.Commands.UpdateJobSkill
{
    public class UpdateJobSkillCommandHandler(
        IJobsSkillsRepository jobsSkillsRepository,
        IJobsRepository jobsRepository,
        ISkillsRepository skillsRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateJobSkillCommand, ErrorOr<JobsSkillsDto>>
    {
        private readonly IJobsSkillsRepository _jobsSkillsRepository = jobsSkillsRepository;
        private readonly IJobsRepository _jobsRepository = jobsRepository;
        private ISkillsRepository _skillsRepository = skillsRepository;
        private IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<JobsSkillsDto>> Handle(UpdateJobSkillCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var oldJobSkill = await _jobsSkillsRepository.GetJobSkillByIdAsync(command.oldJobSkillId);

                if (oldJobSkill == null)
                {
                    return Error.Validation(description: "Job and skill do not exist together.");
                }

                if (await _jobsRepository.GetJobByIdAsync(command.jobId) is not Job job)
                {
                    return Error.Validation(description: "Target job does not exist.");
                }

                if (oldJobSkill.JobId != job.Id &&
                    await _jobsSkillsRepository.GetOldJobAndJobSkillsCountAsync(oldJobSkill.JobId, job.Id) > 3)
                {
                    return Error.Validation(description: "Updating this job skills would make its skills go above 3.");
                }

                if (await _skillsRepository.GetSkillByIdAsync(command.skillId) is not Skill skill)
                {
                    return Error.Validation(description: "Target skill does not exist.");
                }

                if (await _jobsSkillsRepository.JobSkillExistsByJobAndSkillIdsAsync(job.Id, skill.Id) == true)
                {
                    return Error.Validation(description: "Updating course skill will cause duplicates.");
                }

                var newJobSkill = oldJobSkill.UpdateJobSkill(
                    jobId: job.Id,
                    skillId: skill.Id,
                    weight: command.weight ?? oldJobSkill.Weight);

                if (newJobSkill.IsError)
                {
                    return newJobSkill.Errors;
                }

                _jobsSkillsRepository.UpdateJobsSkills(oldJobSkill, newJobSkill.Value);

                await _unitOfWork.CommitChangesAsync();

                return new JobsSkillsDto
                {
                    JobSkillId = newJobSkill.Value.Id,
                    DepartmentId = job.DepartmentId,
                    JobId = newJobSkill.Value.JobId,
                    JobTitle = job.Title,
                    SkillId = newJobSkill.Value.Id,
                    Skill = skill.SkillName,
                    Weight = newJobSkill.Value.Weight,

                    AffectedId = newJobSkill.Value.Id
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
