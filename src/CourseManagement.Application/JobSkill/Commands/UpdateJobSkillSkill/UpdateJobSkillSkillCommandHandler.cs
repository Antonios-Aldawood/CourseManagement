using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.JobSkill.Common.Dto;
using CourseManagement.Domain.JobsSkills;

namespace CourseManagement.Application.JobSkill.Commands.UpdateJobSkillSkill
{
    public class UpdateJobSkillSkillCommandHandler(
        IJobsSkillsRepository jobsSkillsRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateJobSkillSkillCommand, ErrorOr<List<JobsSkillsDto>>>
    {
        private readonly IJobsSkillsRepository _jobsSkillsRepository = jobsSkillsRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<List<JobsSkillsDto>>> Handle(UpdateJobSkillSkillCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var oldSkillAndSkillWithJobs = await _jobsSkillsRepository.GetOldSkillAndSkillWithSkillJobsByIdAsync(command.oldSkillId, command.skillId ?? 0);

                var oldSkillAndJobs = oldSkillAndSkillWithJobs
                    .Where(oSASWJ => oSASWJ.SkillId == command.oldSkillId)
                    .FirstOrDefault();

                if (oldSkillAndJobs == null ||
                    oldSkillAndJobs.SkillId == null)
                {
                    return Error.Validation(description: "Skill does not exist.");
                }

                if (oldSkillAndJobs.JobsSkill == null ||
                    oldSkillAndJobs.JobsSkill.Count == 0)
                {
                    return Error.Validation(description: "Skill was given to no jobs.");
                }

                var skillAndJobs = oldSkillAndSkillWithJobs
                    .Where(oSASWJ => oSASWJ.SkillId == command.skillId)
                    .FirstOrDefault();

                if (command.skillId != null &&
                    (skillAndJobs == null || skillAndJobs.SkillId == null))
                {
                    return Error.Validation(description: "Target skill not found.");
                }

                if (command.skillId != null &&
                    skillAndJobs!.JobsSkill != null &&
                    skillAndJobs!.JobsSkill.Count != 0)
                {
                    var jobIds = new HashSet<int>(skillAndJobs!.JobsSkill.Select(js => js.SkillId));
                    bool duplicates = oldSkillAndJobs.JobsSkill.Any(oCS => jobIds.Contains(oCS.SkillId));

                    if (duplicates == true)
                    {
                        return Error.Validation(description: "Updating job skill will cause duplicates.");
                    }
                }

                List<JobsSkillsDto> jobsSkillsResponse = [];

                foreach (JobsSkills oldJobSkill in oldSkillAndJobs.JobsSkill)
                {
                    var newJobSkill = oldJobSkill.UpdateJobSkill(
                        jobId: oldJobSkill.JobId,
                        skillId: skillAndJobs != null && skillAndJobs.SkillId != null ? skillAndJobs.SkillId ?? 0 : oldJobSkill.SkillId,
                        //skillId: command.skillId != null ? skillAndJobs!.SkillId ?? 0 : oldJobSkill.SkillId,
                        weight: command.weight ?? oldJobSkill.Weight);

                    if (newJobSkill.IsError)
                    {
                        return newJobSkill.Errors;
                    }

                    jobsSkillsResponse.Add(new JobsSkillsDto
                    {
                        JobSkillId = newJobSkill.Value.Id,
                        JobId = newJobSkill.Value.JobId,
                        SkillId = newJobSkill.Value.SkillId,
                        Weight = newJobSkill.Value.Weight,

                        AffectedId = newJobSkill.Value.Id
                    });

                    _jobsSkillsRepository.UpdateJobsSkills(oldJobSkill, newJobSkill.Value);
                }

                await _unitOfWork.CommitChangesAsync();

                return jobsSkillsResponse;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}

/*
                                ////////// WORKING INSIDE TRY //////////
                    ////////// ADD DEPARTMENTS REPOSITORY FOR THIS TO WORK //////////
////////// MORE RESPECTFUL OF DDD BUT THE ONE IN USE DOESN'T BREAK INVARIANTS NOR CONSISTENCY EITHER //////////

                var oldSkill = await _skillsRepository.GetSkillByIdAsync(command.oldSkillId);

                if (oldSkill == null)
                {
                    return Error.Validation(description: "Skill does not exist.");
                }

                var skill = await _skillsRepository.GetSkillByIdAsync(command.skillId ?? 0);
                
                if (skill == null && command.skillId != null)
                {
                    return Error.Validation(description: "Target skill not found.");
                }

                var skillJobs = await _jobsSkillsRepository.GetJobsSkillsBySkillIdAsync(oldSkill.Id);

                if (skillJobs is null ||
                    skillJobs.Count == 0)
                {
                    return Error.Validation(description: "Skill was given to no jobs.");
                }

                List<JobsSkillsDto> jobsSkillsResponse = [];

                foreach(JobsSkills oldJobSkill in skillJobs)
                {
                    var newJobSkill = oldJobSkill.UpdateJobSkill(
                        jobId: oldJobSkill.JobId,
                        skillId: skill != null ? skill.Id : oldJobSkill.SkillId,
                        weight: command.weight ?? oldJobSkill.Weight);

                    if (newJobSkill.IsError)
                    {
                        return newJobSkill.Errors;
                    }

                    jobsSkillsResponse.Add(new JobsSkillsDto
                    {
                        JobSkillId = newJobSkill.Value.Id,
                        JobId = newJobSkill.Value.JobId,
                        SkillId = newJobSkill.Value.SkillId,
                        Weight = newJobSkill.Value.Weight,

                        AffectedId = newJobSkill.Value.Id
                    });

                    _jobsSkillsRepository.UpdateJobsSkills(oldJobSkill, newJobSkill.Value);
                }

                await _unitOfWork.CommitChangesAsync();

                return jobsSkillsResponse;
*/