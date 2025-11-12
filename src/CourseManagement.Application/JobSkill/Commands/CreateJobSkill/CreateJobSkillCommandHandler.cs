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
using CourseManagement.Domain.JobsSkills;

namespace CourseManagement.Application.JobSkill.Commands.CreateJobSkill
{
    public class CreateJobSkillCommandHandler(

        IJobsSkillsRepository jobsSkillsRepository,
        IJobsRepository jobsRepository,
        ISkillsRepository skillsRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<CreateJobSkillCommand, ErrorOr<JobsSkillsDto>>
    {
        private readonly IJobsSkillsRepository _jobsSkillsRepository = jobsSkillsRepository;
        private readonly IJobsRepository _jobsRepository = jobsRepository;
        private readonly ISkillsRepository _skillsRepository = skillsRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<JobsSkillsDto>> Handle(CreateJobSkillCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (await _jobsRepository.GetJobByExactNameAsync(command.jobTitle) is not Job job)
                {
                    return Error.Validation(description: "Job does not exist.");
                }

                if (await _skillsRepository.GetSkillByExactSkillNameAsync(command.skillSkillName) is not Skill skill)
                {
                    return Error.Validation(description: "Skill does not exist.");
                }

                List<JobsSkills> jobsSkills = await _jobsSkillsRepository.GetJobsSkillsByJobIdAsync(job.Id);

                if (jobsSkills is not null &&
                    jobsSkills.Count >= 3)
                {
                    return Error.Validation(description: "Job can not require more than 3 jobs.");
                }

                if (jobsSkills is not null &&
                    jobsSkills.Where(js => js.JobId == job.Id && js.SkillId == skill.Id).FirstOrDefault() is not null)
                {
                    return Error.Validation(description: "Job already has skill.");
                }

                var jobSkill = JobsSkills.CreateJobSkill(
                    jobId: job.Id,
                    skillId: skill.Id,
                    weight: command.weight);

                if (jobSkill.IsError)
                {
                    return jobSkill.Errors;
                }

                await _jobsSkillsRepository.AddJobsSkillsAsync(jobSkill.Value);

                await _unitOfWork.CommitChangesAsync();

                return new JobsSkillsDto
                {
                    JobSkillId = jobSkill.Value.Id,
                    DepartmentId = job.DepartmentId,
                    JobId = jobSkill.Value.JobId,
                    JobTitle = job.Title,
                    SkillId = jobSkill.Value.Id,
                    Skill = skill.SkillName,
                    Weight = jobSkill.Value.Weight,

                    AffectedId = jobSkill.Value.Id
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
