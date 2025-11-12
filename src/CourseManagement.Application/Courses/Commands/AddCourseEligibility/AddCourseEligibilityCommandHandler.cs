using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Domain.Courses;
using CourseManagement.Domain.Departments;
using CourseManagement.Application.Jobs.Common.Dto;

namespace CourseManagement.Application.Courses.Commands.AddCourseEligibility
{
    public class AddCourseEligibilityCommandHandler(
        ICoursesRepository coursesRepository,
        IDepartmentsRepository departmentsRepository,
        IJobsRepository jobsRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<AddCourseEligibilityCommand, ErrorOr<List<EligibilityDto>>>
    {
        private readonly ICoursesRepository _coursesRepository = coursesRepository;
        private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
        private readonly IJobsRepository _jobsRepository = jobsRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<List<EligibilityDto>>> Handle(AddCourseEligibilityCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (await _coursesRepository.GetCourseByExactSubjectAsync(command.courseSubject) is not Course course)
                {
                    return Error.Validation(description: "Course not found.");
                }

                List<EligibilityDto> eligibilityResponse = [];

                if (command.position == null &&
                    command.department == null &&
                    command.job == null)
                {
                    course.SpecifyCourseEligibilityForAll();

                    await _unitOfWork.CommitChangesAsync();

                    EligibilityDto eligibility = new EligibilityDto
                    {
                        EligibilityId = 0,
                        Course = course.Subject,
                        Key = "All",
                        Value = 0
                    };

                    eligibility.AffectedId = eligibility.EligibilityId;

                    eligibilityResponse.Add(eligibility);

                    return eligibilityResponse;
                }

                List<Department> departments = [];

                List<JobDto> jobs = [];

                if (command.department != null &&
                    command.departmentIds != null)
                {
                    departments = await _departmentsRepository.GetAllDepartmentsForEligibilityAsync(command.departmentIds);

                    if (departments.Count != command.departmentIds.Count)
                    {
                        return Error.Validation(description: "Some departments not found.");
                    }

                    jobs = await _jobsRepository.GetDepartmentsJobsAsync(command.departmentIds);

                    //bool foundInAnyDepartment = jobs.Any(job =>
                    //        departments.Any(department => job.Department == department.Name));

                    //if (!foundInAnyDepartment)
                    //{
                    //    return Error.Validation(description: "No job found in any department.");
                    //}

                    if (command.job != null &&
                        command.jobIds != null &&
                        jobs.Any(job => command.jobIds.Any(jId => job.JobId == jId)) == false)
                    {
                        return Error.Validation(description: "Jobs do not belong to any entered department.");
                    }
                }

                if (command.position != null &&
                    command.positionIds != null)
                {
                    foreach (int value in command.positionIds)
                    {
                        var positionEli = course.SpecifyCourseEligibility(
                            key: command.position,
                            value: value);

                        if (positionEli.IsError)
                        {
                            return positionEli.Errors;
                        }

                        if (course.IsForAll)
                        {
                            course.IsForAll = false;
                        }

                        await _unitOfWork.CommitChangesAsync();

                        EligibilityDto positionDto = new EligibilityDto
                        {
                            EligibilityId = positionEli.Value.Id,
                            Course = course.Subject,
                            Key = positionEli.Value.Key,
                            Value = positionEli.Value.Value,
                        };

                        positionDto.AffectedId = positionEli.Value.Id;

                        eligibilityResponse.Add(positionDto);
                    }
                }

                if (command.department != null &&
                    command.departmentIds != null)
                {
                    foreach (int value in command.departmentIds)
                    {
                        var departmentEli = course.SpecifyCourseEligibility(
                            key: command.department,
                            value: value);

                        if (departmentEli.IsError)
                        {
                            return departmentEli.Errors;
                        }

                        if (course.IsForAll)
                        {
                            course.IsForAll = false;
                        }

                        await _unitOfWork.CommitChangesAsync();

                        EligibilityDto departmentDto = new EligibilityDto
                        {
                            EligibilityId = departmentEli.Value.Id,
                            Course = course.Subject,
                            Key = departmentEli.Value.Key,
                            Value = departmentEli.Value.Value,
                        };

                        departmentDto.AffectedId = departmentEli.Value.Id;

                        eligibilityResponse.Add(departmentDto);
                    }

                    if (command.job != null &&
                        command.jobIds != null &&
                        departments != null)
                    {
                        List<int> savedIdsToRemove = [];

                        foreach (int jobValue in command.jobIds)
                        {
                            var jobEli = course.SpecifyCourseEligibility(
                                command.job!,
                                jobValue);

                            if (jobEli.IsError)
                            {
                                return jobEli.Errors;
                            }

                            if (course.IsForAll)
                            {
                                course.IsForAll = false;
                            }

                            savedIdsToRemove.Add(jobValue);

                            await _unitOfWork.CommitChangesAsync();

                            eligibilityResponse.Add(new EligibilityDto
                            {
                                EligibilityId = jobEli.Value.Id,
                                Course = course.Subject,
                                Key = jobEli.Value.Key,
                                Value = jobEli.Value.Value,
                                AffectedId = jobEli.Value.Id
                            });
                        }

                        foreach (int id in savedIdsToRemove)
                        {
                            command.jobIds.Remove(id);
                        }
                    }
                }

                if (eligibilityResponse.Count == 0 &&
                    course.IsForAll != true)
                {
                    return Error.Validation(description: "No eligibility could be added.");
                }

                return eligibilityResponse;
            }
            catch (Exception ex)
            {
                return Error.Validation(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}

/*
                if (command.keys.Count != command.values.Count)
                {
                    return Error.Validation(description: "Course eligibilities must belong to a known key.");
                }

                if (await _coursesRepository.GetCourseByExactSubjectAsync(command.courseSubject) is not Course course)
                {
                    return Error.Validation(description: "Course not found.");
                }

                List<EligibilityDto> eligibilityResponse = [];

                if (command.keys.Any(k => k == "Position"))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        foreach (int innerValue in command.values[i])
                        {
                            if (innerValue != 1 &&
                                innerValue != 2 &&
                                innerValue != 3)
                            {
                                return Error.Validation(description: "Eligibility should belong to an already existing element.");
                            }

                            var newEligibility = course.SpecifyCourseEligibility(
                                key: "Position",
                                value: innerValue);

                            if (newEligibility.IsError)
                            {
                                return newEligibility.Errors;
                            }

                            EligibilityDto dto = new EligibilityDto
                            {
                                EligibilityId = newEligibility.Value.Id,
                                Course = course.Subject,
                                Key = newEligibility.Value.Key,
                                Value = newEligibility.Value.Value
                            };

                            dto.AffectedId = newEligibility.Value.Id;

                            eligibilityResponse.Add(dto);
                        }
                    }
                }

                for (int i = 0; i < 3; i++)
                {
                    foreach (int innerValue in command.values[i])
                    {
                        if (await _departmentsRepository.ExistsAsync(innerValue) == false &&
                            await _departmentsRepository.JobExistsAsync(innerValue) == false &&
                            innerValue != 1 &&
                            innerValue != 2 &&
                            innerValue != 3)
                        {
                            return Error.Validation(description: "Eligibility should belong to an already existing element.");
                        }

                        var newEligibility = course.SpecifyCourseEligibility(
                            key: command.keys[i],
                            value: innerValue);
                        
                        if (newEligibility.IsError)
                        {
                            return newEligibility.Errors;
                        }

                        EligibilityDto dto = new EligibilityDto
                        {
                            EligibilityId = newEligibility.Value.Id,
                            Course = course.Subject,
                            Key = newEligibility.Value.Key,
                            Value = newEligibility.Value.Value
                        };

                        dto.AffectedId = newEligibility.Value.Id;

                        eligibilityResponse.Add(dto);
                    }
                }

                return eligibilityResponse;
*/


/*
//inside the first foreach loop, right after the: if (command.job != null && command.jobIds != null && departments != null) condition.

foreach (Department department in departments)
{
    var jobIdsInDepartment = department.Jobs!.Select(j => j.Id).ToHashSet();
    var commandJobIds = command.jobIds.ToHashSet();

    bool allJobsIncluded = commandJobIds.SetEquals(jobIdsInDepartment);

    if (allJobsIncluded)
    {
        continue;
    }
    else
    {
        foreach (int jobValue in command.jobIds)
        {
            if (!jobIdsInDepartment.Contains(jobValue))
            {
                return Error.Validation(description: "Job not found in department.");
            }

            var jobEli = course.SpecifyCourseEligibility(command.job!, jobValue);
            if (jobEli.IsError) return jobEli.Errors;

            eligibilityResponse.Add(new EligibilityDto
            {
                EligibilityId = jobEli.Value.Id,
                Course = course.Subject,
                Key = jobEli.Value.Key,
                Value = jobEli.Value.Value,
                AffectedId = jobEli.Value.Id
            });
        }
    }
}
*/