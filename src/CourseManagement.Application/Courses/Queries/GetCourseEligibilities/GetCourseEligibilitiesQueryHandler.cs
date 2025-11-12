using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;

namespace CourseManagement.Application.Courses.Queries.GetCourseEligibilities
{
    public class GetCourseEligibilitiesQueryHandler(
        ICoursesRepository coursesRepository,
        IDepartmentsRepository departmentsRepository,
        IJobsRepository jobsRepository
        ) : IRequestHandler<GetCourseEligibilitiesQuery, ErrorOr<List<EligibilityValuesDto>>>
    {
        private readonly ICoursesRepository _coursesRepository = coursesRepository;
        private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
        private readonly IJobsRepository _jobsRepository = jobsRepository;

        public async Task<ErrorOr<List<EligibilityValuesDto>>> Handle(GetCourseEligibilitiesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<EligibilityValuesDto> eligibilitiesResponse = [];

                //Course existence checking.
                var course = await _coursesRepository.GetCourseByExactSubjectAsync(query.subject);
                if (course == null)
                {
                    return Error.Validation(description: "Course not found.");
                }

                //Eligibilities if course is available for all.
                if (course.IsForAll)
                {
                    EligibilityValuesDto eligibilityValues = new EligibilityValuesDto
                    {
                        EligibilityId = 0,
                        Course = course.Subject,
                        Key = "All",
                        Value = "All"
                    };

                    eligibilityValues.AffectedId = eligibilityValues.EligibilityId;

                    eligibilitiesResponse.Add(eligibilityValues);

                    return eligibilitiesResponse;
                }

                //Eligibilities retrieval and existence checking.
                List<EligibilityDto> eligibilities = await _coursesRepository.GetAllEligibilitiesForCourseBySubject(course.Subject);

                if (eligibilities is null ||
                    eligibilities.Count == 0)
                {
                    return Error.Validation(description: "Course has no eligibilities.");
                }

                //Position, Job, and Department, eligibilities values ascertaining.
                List<int> jobIds = eligibilities
                    .Where(e => e.Key == "Job")
                    .Select(e => e.Value)
                    .ToList();

                List<int> departmentIds = eligibilities
                    .Where(e => e.Key == "Department")
                    .Select(e => e.Value)
                    .ToList();

                Dictionary<int, string> departments = await _departmentsRepository.GetDepartmentsByIdsAsync(departmentIds);

                Dictionary<int, string> jobs = await _jobsRepository.GetJobsByIdsAsync(jobIds);
                
                eligibilitiesResponse = eligibilities.Select(e =>
                {
                    var value = e.Key switch
                    {
                        "Department" => departments.TryGetValue(e.Value, out var departmentName) ? departmentName : "Unknown Department",
                        "Job" => jobs.TryGetValue(e.Value, out var jobTitle) ? jobTitle : "Unknown Job",
                        "Position" => e.Value switch
                        {
                            1 => "CEO",
                            2 => "CTO",
                            3 => "Director",
                            4 => "Manager",
                            5 => "Supervisor",
                            6 => "Specialist",
                            7 => "Intern",
                            _ => "Unknown Position"
                        },
                        _ => "Invalid Key"
                    };

                    return new EligibilityValuesDto
                    {
                        EligibilityId = e.EligibilityId,
                        Course = e.Course,
                        Key = e.Key,
                        Value = value
                    };
                }).ToList();

                eligibilitiesResponse.ForEach(eR => eR.AffectedId = eR.EligibilityId);

                return eligibilitiesResponse;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}

/*
                foreach (EligibilityDto eligibility in eligibilities)
                {
                    if (eligibility.Key == "Position")
                    {
                        switch (eligibility.Value)
                        {
                            
                        }
                    }

                    if (eligibility.Key == "Department")
                    {

                    }
                }
*/