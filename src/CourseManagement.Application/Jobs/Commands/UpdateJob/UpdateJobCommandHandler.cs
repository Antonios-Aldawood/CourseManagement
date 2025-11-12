using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Jobs.Common.Dto;
using CourseManagement.Domain.Jobs;

namespace CourseManagement.Application.Jobs.Commands.UpdateJob
{
    public class UpdateJobCommandHandler(
        IDepartmentsRepository departmentsRepository,
        IJobsRepository jobsRepository,
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateJobCommand, ErrorOr<JobDto>>
    {
        private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
        private readonly IJobsRepository _jobsRepository = jobsRepository;
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<JobDto>> Handle(UpdateJobCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var oldJob = await _jobsRepository.GetJobByExactNameAsync(command.oldJobTitle);

                if (oldJob is not Job existingJob)
                {
                    return Error.Validation(description: "Job not found.");
                }

                if (command.title != null &&
                    command.title != oldJob.Title &&
                    await _jobsRepository.JobTitleExistsAsync(command.title) == true)
                {
                    return Error.Validation(description: "Job title already taken.");
                }

                if (command.departmentId != null &&
                    await _departmentsRepository.ExistsAsync(command.departmentId ?? 0) == false)
                {
                    return Error.Validation(description: "Target department not found.");
                }

                if (command.departmentId != null &&
                    command.departmentId != oldJob.DepartmentId &&
                    await _usersRepository.UsersHaveUppersInDepartmentAsync(oldJob.DepartmentId, oldJob.Id) == true)
                {
                    return Error.Validation(description: "Shouldn't change job's department, because it has users that are uppers from the old department.");
                }

                var newJob = existingJob.UpdateJob(
                    title: command.title ?? existingJob.Title,
                    minSalary: command.minSalary ?? existingJob.MinSalary,
                    maxSalary: command.maxSalary ?? existingJob.MaxSalary,
                    description: command.description ?? existingJob.Description,
                    departmentId: command.departmentId ?? existingJob.DepartmentId
                );

                if (newJob.IsError)
                {
                    return newJob.Errors;
                }

                await _unitOfWork.CommitChangesAsync();

                return new JobDto
                {
                    JobId = newJob.Value.Id,
                    Title = newJob.Value.Title,
                    MinSalary = newJob.Value.MinSalary,
                    MaxSalary = newJob.Value.MaxSalary,
                    Description = newJob.Value.Description,
                    Department = newJob.Value.DepartmentId.ToString(),
                    AffectedId = newJob.Value.Id
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}

/*

//////////////// WOULD HAVE REPLACED THE SECOND AND THIRD VISTS TO THE DB, IF THEY WERE AUTONOMOUS ////////////////
    if (command.departmentId != null)
    {
        int existingTitleOrNotExistingDepartment =
        await _departmentsRepository.DepartmentAndJobExistsAsync(command.oldDepartmentName, command.oldJobTitle);

        if (existingTitleOrNotExistingDepartment == 1)
        {
            return Error.Validation(description: "Department not found.");
        }

        if (existingTitleOrNotExistingDepartment == 2)
        {
            return Error.Validation(description: "Job title already taken.");
        }
    }



                JobDto? jobDto = null;

                foreach (Job existingJob in oldDepartment.Jobs!)
                {
                    if (existingJob.Title == command.oldJobTitle)
                    {
                        var newJob = oldDepartment.UpdateDepartmentJob(
                            oldJob: existingJob,
                            title: command.title ?? existingJob.Title,
                            minSalary: command.minSalary ?? existingJob.MinSalary,
                            maxSalary: command.maxSalary ?? existingJob.MaxSalary,
                            description: command.description ?? existingJob.Description,
                            departmentId: command.departmentId ?? oldDepartment.Id);

                        if (newJob.IsError)
                        {
                            return newJob.Errors;
                        }

                        await _unitOfWork.CommitChangesAsync();

                        jobDto = new JobDto
                        {
                            JobId = newJob.Value.Id,
                            Title = newJob.Value.Title,
                            MinSalary = newJob.Value.MinSalary,
                            MaxSalary = newJob.Value.MaxSalary,
                            Description = newJob.Value.Description,
                            Department = oldDepartment.Name
                        };

                        jobDto.AffectedId = newJob.Value.Id;

                        break;
                    }
                    
                    if (existingJob == oldDepartment.Jobs.Last() &&
                        existingJob.Title != command.oldJobTitle)
                    {
                        return Error.Validation(description: "Job does not exist.");
                    }
                }

                // Won't be returned null, because if anything wrong already happened, it would have already thrown an exception.
                return jobDto!;
*/

/*
                if (command.departmentId != null &&
                    await _departmentsRepository.DepartmentExistsWithUsersAsync(command.departmentId ?? 0) == true)
                {
                    return Error.Validation(description: "Can not update job's department, for it has users that can be each other's uppers.")
                }
*/