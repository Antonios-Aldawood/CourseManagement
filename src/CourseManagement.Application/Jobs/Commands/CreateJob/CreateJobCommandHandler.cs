using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Domain.Jobs;
using CourseManagement.Domain.Departments;
using CourseManagement.Application.Jobs.Common.Dto;

namespace CourseManagement.Application.Jobs.Commands.CreateJob
{
    public class CreateJobCommandHandler(
        IJobsRepository jobsRepository,
        IDepartmentsRepository departmentsRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<CreateJobCommand, ErrorOr<JobDto>>
    {
        private readonly IJobsRepository _jobsRepository = jobsRepository;
        private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<JobDto>> Handle(CreateJobCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (await _jobsRepository.JobTitleExistsAsync(command.title) == true)
                {
                    return Error.Validation(description: "Job title already taken.");
                }

                if (await _departmentsRepository.GetByExactNameAsync(command.departmentName) is not Department department)
                {
                    return Error.Validation(description: "Department not found.");
                }

                var job = Job.CreateJob(
                    title: command.title,
                    minSalary: command.minSalary,
                    maxSalary: command.maxSalary,
                    description: command.description,
                    departmentId: department.Id);

                if (job.IsError)
                {
                    return job.Errors;
                }

                await _jobsRepository.AddJobAsync(job.Value);

                await _unitOfWork.CommitChangesAsync();

                JobDto jobDto = new JobDto
                {
                    JobId = job.Value.Id,
                    Title = job.Value.Title,
                    MinSalary = job.Value.MinSalary,
                    MaxSalary = job.Value.MaxSalary,
                    Description = job.Value.Description,
                    Department = department.Name
                };

                jobDto.AffectedId = job.Value.Id;

                return jobDto;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
