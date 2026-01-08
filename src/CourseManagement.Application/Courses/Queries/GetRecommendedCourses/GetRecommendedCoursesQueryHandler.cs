using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Application.Common.Behaviors;
using CourseManagement.Application.Users.Common.Dto;
using CourseManagement.Domain.Services;

namespace CourseManagement.Application.Courses.Queries.GetRecommendedCourses
{
    public class GetRecommendedCoursesQueryHandler(
        IUsersRepository usersRepository,
        ICoursesRepository coursesRepository
        ) : IRequestHandler<GetRecommendedCoursesQuery, ErrorOr<List<CourseDto>>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly ICoursesRepository _coursesRepository = coursesRepository;

        public async Task<ErrorOr<List<CourseDto>>> Handle(GetRecommendedCoursesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<CourseDto> coursesResponse = [];

                List<UserJobSkillsDto> userJobSkills = await _usersRepository.GetUserWithJobAndJobSkillsAsync(query.alias);

                if (userJobSkills == null ||
                    userJobSkills.Count == 0)
                {
                    return Error.Validation(description: "User either not found, or their job has no skills.");
                }

                var authenticatedAlias = IdentityBehavior.CheckIfAuthenticationAliasMatch(query.headers, query.alias);
                if (authenticatedAlias != Result.Success)
                {
                    return authenticatedAlias.Errors;
                }

                int positionId = UserPositionCourseEligibilityService.ValidEligibilityPosition(userJobSkills.First().Position);

                List<CourseSkillsDto> courseSkills = await _coursesRepository.GetCourseWithCourseSkillsByEligibilities(
                    positionId: positionId,
                    departmentId: userJobSkills.First().DepartmentId,
                    jobId: userJobSkills.First().JobId);

                if (courseSkills == null ||
                    courseSkills.Count == 0)
                {
                    //return Error.Validation(description: "No course available to recommended.");
                    return coursesResponse;
                }

                /// Match The Courses And Jobs That Share A Skill, And Rate Them By These Skills Highest Weights.

                Dictionary<int, double> courseScoreMap = [];

                /*
                var userSkillMap = userJobSkills
                    .GroupBy(s => s.SkillName)
                    .ToDictionary(g => g.Key, g => g.ToList());
                */

                foreach (var courseSkill in courseSkills)
                {
                    // Find matching user skills for this course skill
                    var matchingUserSkills = userJobSkills
                        .Where(u => u.SkillName == courseSkill.SkillName);

                    foreach (var userSkill in matchingUserSkills)
                    {
                        // Calculate a relevance score
                        double score = courseSkill.Weight * userSkill.Weight;

                        if (courseScoreMap.ContainsKey(courseSkill.CourseId))
                        {
                            courseScoreMap[courseSkill.CourseId] += score;
                        }
                        else
                        {
                            courseScoreMap[courseSkill.CourseId] = score;
                        }
                    }
                }

                // Get distinct course info for top scoring ones
                List<CourseSkillsDto> bestCourses = courseSkills
                    .Where(cs => courseScoreMap.ContainsKey(cs.CourseId))
                    .GroupBy(cs => cs.CourseId)
                    .Select(group => group.First()) // avoid duplicates
                    .OrderByDescending(cs => courseScoreMap[cs.CourseId]) // rank by calculated score
                    //.DistinctBy(cs => cs.CourseId)
                    //.OrderByDescending(cs => finalScoreMap[cs.CourseId])
                    .ToList();

                // Prepare final DTO list
                coursesResponse = bestCourses.Select(c =>
                    CourseDto.AddCourseDtoByAttributes(
                        courseId: c.CourseId,
                        subject: c.Subject,
                        description: c.Description)
                ).ToList();

                return coursesResponse;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}

/*
                coursesResponse = courseSkills.Select(c =>
                {
                    foreach (UserJobSkillsDto userSkill in userJobSkills)
                    {
                        if (c.SkillName == userSkill.SkillName)
                        {
                            return CourseDto.AddCourseDtoByAttributes(
                                courseId: c.CourseId,
                                subject: c.Subject,
                                description: c.Description);
                        }

                        continue;
                    }

                    return new List<CourseDto>();
                }).ToList();




                foreach (CourseSkillsDto courseSkill in courseSkills.OrderByDescending(cs => cs.Weight).Distinct().ToList())
                {
                    foreach (UserJobSkillsDto userJobSkill in userJobSkills.OrderByDescending(uJS => uJS.Weight).Distinct().ToList())
                    {
                        if (courseSkill.SkillName == userJobSkill.SkillName)
                        {
                            coursesResponse.Add(CourseDto.AddCourseDtoByAttributes(
                                courseId: courseSkill.CourseId,
                                subject: courseSkill.Subject,
                                description: courseSkill.Description));
                        }
                    }
                }



                List<CourseSkillsDto> caughtCourseSkills = [];
                List<UserJobSkillsDto> caughtUserJobSkills = [];

                foreach (CourseSkillsDto courseSkill in courseSkills)
                {
                    foreach (UserJobSkillsDto userJobSkill in userJobSkills)
                    {
                        if (courseSkill.SkillName == userJobSkill.SkillName)
                        {
                            caughtCourseSkills.Add(courseSkill);
                            caughtUserJobSkills.Add(userJobSkill);
                        }
                    }
                }

                if (caughtCourseSkills is null || caughtUserJobSkills.Count == 0 ||
                    caughtUserJobSkills is null || caughtUserJobSkills.Count == 0)
                {
                    return Error.Validation(description: "User job has no courses that match its skillset.");
                }

                caughtCourseSkills.OrderByDescending(cs => cs.Weight);
                caughtUserJobSkills.OrderByDescending(uJS => uJS.Weight);
*/

/*
        IUsersSearchesRepository usersSearchesRepository,
        private readonly IUsersSearchesRepository _usersSearchesRepository = usersSearchesRepository;

        private (double alpha, double beta) GetAlphaBeta(int searchCount)
        {
            double alpha = 0.6;
            double beta = 0.4;

            if (searchCount >= 3 && searchCount < 7)
            {
                int extra = searchCount - 3;
                for (int i = 0; i < extra; i++)
                {
                    alpha += 0.3;
                    beta += 0.4;
                }
            }
            else if (searchCount >= 7)
            {
                alpha = 1.7;
                beta = 1.9;
            }
            else
            {
                alpha = 0.8;
                beta = 0.2;
            }

            return (alpha, beta);
        }
*/

/*
================================= AFTER THE SKILLS BASED ALGO ======================================
/// Integrate User Searched Courses Into The Recommendation Algorithm, By Matching By Skill.
// Fetch searches
var userSearches = await _usersSearchesRepository.GetUsersSearchesDtoAsync(userJobSkills.First().UserId);

// Group searches by course, so we can count repetitions
var groupedSearches = userSearches
    .GroupBy(us => us.SearchedCourseId)
    .ToDictionary(c => c.Key, c => c.ToList());

// Map courseId → search-based score
Dictionary<int, double> searchScoreMap = [];

foreach (var userSearch in groupedSearches)
{
    var courseId = userSearch.Key;
    var searches = userSearch.Value;
    int searchCount = searches.Count;

    // Weight knobs
    var (alpha, beta) = GetAlphaBeta(searchCount);

    // For each searched skill, find matching skills in candidate courses
    var searchedSkills = searches.Select(s => new { s.SearchedCourseSkill, s.SearchedCourseSkillWeight });

    foreach (var course in courseSkills)
    {
        foreach (var searchedSkill in searchedSkills)
        {
            if (course.SkillName == searchedSkill.SearchedCourseSkill)
            {
                double score = course.Weight * searchedSkill.SearchedCourseSkillWeight;
                if (!searchScoreMap.ContainsKey(course.CourseId))
                {
                    searchScoreMap[course.CourseId] = 0;
                }
                else
                {
                    searchScoreMap[course.CourseId] += beta * score; // Only apply β to search component
                }
            }
        }
    }
}

Dictionary<int, double> finalScoreMap = [];

foreach (var courseId in courseScoreMap.Keys.Union(searchScoreMap.Keys))
{
    double jobScore = courseScoreMap.ContainsKey(courseId) ? courseScoreMap[courseId] : 0;
    double searchScore = searchScoreMap.ContainsKey(courseId) ? searchScoreMap[courseId] : 0;

    // Default alpha/beta from job-only perspective (user may not have searches for all courses)
    var (alpha, beta) = GetAlphaBeta(
        groupedSearches.ContainsKey(courseId) ? groupedSearches[courseId].Count : 0
    );

    double finalScore = alpha * jobScore + searchScore;
    finalScoreMap[courseId] = finalScore;
}
*/
