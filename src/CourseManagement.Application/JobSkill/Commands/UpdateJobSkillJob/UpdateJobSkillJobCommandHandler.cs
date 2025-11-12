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

namespace CourseManagement.Application.JobSkill.Commands.UpdateJobSkillJob
{
    public class UpdateJobSkillJobCommandHandler(
        IJobsSkillsRepository jobsSkillsRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateJobSkillJobCommand, ErrorOr<List<JobsSkillsDto>>>
    {
        private readonly IJobsSkillsRepository _jobsSkillsRepository = jobsSkillsRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<List<JobsSkillsDto>>> Handle(UpdateJobSkillJobCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var oldJobAndJobWithSkills = await _jobsSkillsRepository.GetOldJobAndJobWithJobSkillsByIdAsync(command.oldJobId, command.jobId ?? 0);

                var oldJobAndSkills = oldJobAndJobWithSkills
                    .Where(oJAJWS => oJAJWS.JobId == command.jobId)
                    .FirstOrDefault();

                if (oldJobAndSkills == null ||
                    oldJobAndSkills.JobId == null)
                {
                    return Error.Validation(description: "Job does not exist.");
                }

                if (oldJobAndSkills.JobSkills == null ||
                    oldJobAndSkills.JobSkills.Count == 0)
                {
                    return Error.Validation(description: "Job has no skills.");
                }

                var jobAndSkills = oldJobAndJobWithSkills
                    .Where(oJAJWS => oJAJWS.JobId == command.jobId)
                    .FirstOrDefault();

                if (command.jobId != null &&
                    (jobAndSkills == null || jobAndSkills.JobId == null))
                {
                    return Error.Validation(description: "Target job not found.");
                }

                if (command.jobId != null &&
                    jobAndSkills!.JobSkills != null &&
                    jobAndSkills.JobSkills.Count != 0 &&
                    oldJobAndSkills.JobSkills
                        .Any(oJS => jobAndSkills!.JobSkills.Any(js => oJS.SkillId == js.SkillId)) == true)
                {
                    return Error.Validation(description: "Updating job skill will cause duplicates.");
                }

                if (command.jobId != null &&
                    jobAndSkills!.JobSkills?.Count + oldJobAndSkills.JobSkills.Count > 3)
                {
                    return Error.Validation(description: "Updating this job skills would make its skills go above 3.");
                }

                List<JobsSkillsDto> jobsSkillsResponse = [];

                foreach (JobsSkills oldJobSkill in oldJobAndSkills.JobSkills)
                {
                    var newJobSkill = oldJobSkill.UpdateJobSkill(
                        jobId: jobAndSkills != null && jobAndSkills.JobId != null ? jobAndSkills.JobId ?? 0 : oldJobSkill.JobId,
                        //jobId: command.jobId != null ? jobAndSkills!.JobId ?? 0: oldJobSkill.JobId,
                        skillId: oldJobSkill.SkillId,
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

                var oldDepartment = await _departmentsRepository.GetByIdAsync(command.oldDepartmentId);

                if (oldDepartment == null)
                {
                    return Error.Validation(description: "Department containing job, does not exist");
                }

                var oldJob = oldDepartment.Jobs!.FirstOrDefault(j => j.Id == command.oldJobId);

                if (oldJob is null)
                {
                    return Error.Validation(description: "Job does not exist.");
                }

                var job = await _departmentsRepository.GetJobByIdAsync(command.jobId ?? 0);

                if (job == null && command.jobId != null)
                {
                    return Error.Validation(description: "Target job does not exist.");
                }

                if (command.jobId != null && job != null &&
                    await _jobsSkillsRepository.GetOldJobAndJobSkillsCountAsync(oldJob.Id, job.Id) > 3)
                {
                    return Error.Validation(description: "Updating this job skills would make its skills go above 3."); ;
                }

                var jobSkills = await _jobsSkillsRepository.GetJobsSkillsByJobIdAsync(oldJob.Id);

                if (jobSkills is null ||
                    jobSkills.Count == 0)
                {
                    return Error.Validation(description: "Job has no skills.");
                }

                List<JobsSkillsDto> jobsSkillsResponse = [];

                foreach (JobsSkills oldJobSkill in jobSkills)
                {
                    var newJobSkill = oldJobSkill.UpdateJobSkill(
                        jobId: job != null ? job.Id : oldJobSkill.JobId,
                        skillId: oldJobSkill.SkillId,
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