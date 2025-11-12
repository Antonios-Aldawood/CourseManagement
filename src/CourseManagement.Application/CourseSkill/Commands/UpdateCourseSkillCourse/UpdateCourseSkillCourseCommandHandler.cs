using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.CourseSkill.Common.Dto;
using CourseManagement.Domain.CoursesSkills;

namespace CourseManagement.Application.CourseSkill.Commands.UpdateCourseSkillCourse
{
    public class UpdateCourseSkillCourseCommandHandler(
        ICoursesSkillsRepository coursesSkillsRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateCourseSkillCourseCommand, ErrorOr<List<CoursesSkillsIdsDto>>>
    {
        private readonly ICoursesSkillsRepository _coursesSkillRepository = coursesSkillsRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<List<CoursesSkillsIdsDto>>> Handle(UpdateCourseSkillCourseCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var oldCourseAndCourseWithSkills = await _coursesSkillRepository.GetOldCourseAndCourseWithCourseSkillsByIdAsync(command.oldCourseId, command.courseId ?? 0);

                var oldCourseAndSkills = oldCourseAndCourseWithSkills
                    .Where(oCACWS => oCACWS.CourseId == command.oldCourseId)
                    .FirstOrDefault();

                if (oldCourseAndSkills == null ||
                    oldCourseAndSkills.CourseId == null)
                {
                    return Error.Validation(description: "Course does not exist.");
                }

                if (oldCourseAndSkills.CourseSkills == null ||
                    oldCourseAndSkills.CourseSkills.Count == 0)
                {
                    return Error.Validation(description: "Course has no skills.");
                }

                var courseAndSkills = oldCourseAndCourseWithSkills
                    .Where(oCACWS => oCACWS.CourseId == command.courseId)
                    .FirstOrDefault();

                if (command.courseId != null &&
                    (courseAndSkills == null || courseAndSkills.CourseId == null))
                {
                    return Error.Validation(description: "Target course not found.");
                }

                if (command.courseId != null &&
                    courseAndSkills!.CourseSkills != null &&
                    courseAndSkills!.CourseSkills.Count != 0 &&
                    oldCourseAndSkills.CourseSkills
                        .Any(oCS => courseAndSkills!.CourseSkills.Any(cs => oCS.SkillId == cs.SkillId)) == true)
                {
                    return Error.Validation(description: "Updating course skill will cause duplicates.");
                }

                if (command.courseId != null &&
                    courseAndSkills!.CourseSkills?.Count + oldCourseAndSkills.CourseSkills.Count > 3)
                {
                    return Error.Validation(description: "Updating this course skills would make its skills go above 3.");
                }
                
                List<CoursesSkillsIdsDto> coursesSkillsResponse = [];

                foreach (CoursesSkills oldCourseSkill in oldCourseAndSkills.CourseSkills)
                {
                    var newCourseSkill = oldCourseSkill.UpdateCourseSkill(
                        //courseId: courseAndSkills != null && courseAndSkills.CourseId != null ? courseAndSkills.CourseId ?? 0 : oldCourseSkill.CourseId,
                        courseId: command.courseId != null ? courseAndSkills!.CourseId ?? 0 : oldCourseSkill.CourseId,
                        skillId: oldCourseSkill.SkillId,
                        weight: command.weight ?? oldCourseSkill.Weight);

                    if (newCourseSkill.IsError)
                    {
                        return newCourseSkill.Errors;
                    }

                    coursesSkillsResponse.Add(new CoursesSkillsIdsDto
                    {
                        CourseSkillId = newCourseSkill.Value.Id,
                        CourseId = newCourseSkill.Value.CourseId,
                        SkillId = newCourseSkill.Value.SkillId,
                        Weight = newCourseSkill.Value.Weight,

                        AffectedId = newCourseSkill.Value.Id
                    });

                    _coursesSkillRepository.UpdateCoursesSkills(oldCourseSkill, newCourseSkill.Value);
                }

                await _unitOfWork.CommitChangesAsync();

                return coursesSkillsResponse;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}

/*
                if (command.courseId != null)
                {
                    var updatedCourseSkills = await _coursesSkillRepository.GetCoursesSkillsByCourseIdAsync(command.courseId ?? 0);

                    if (updatedCourseSkills != null &&
                        updatedCourseSkills.Count >= 3)
                    {
                        await _coursesSkillRepository.DeleteCoursesSkillsForCourseAsync(command.courseId ?? 0);
                        return Error.Validation(description: "Updated course skills became more than 3.");
                    }
                }
*/

/*
                    ////////// WORKING INSIDE TRY //////////
        ////////// ADD COURSES REPOSITORY FOR THIS TO WORK //////////
        
                var oldCourse = await _coursesRepository.GetCourseByIdAsync(command.oldCourseId);

                if (oldCourse == null)
                {
                   return Error.Validation(description: "Course does not exist.");
                }

                var course = await _coursesRepository.GetCourseByIdAsync(command.courseId ?? 0);

                if (course == null && command.courseId != null)
                {
                    return Error.Validation(description: "Target course not found.");
                }

                if (command.courseId != null && course != null &&
                    await _coursesSkillRepository.GetOldCourseAndCourseSkillsCountAsync(oldCourse.Id, course.Id) > 3)
                {
                    return Error.Validation(description: "Updating this course skills would make its skills go above 3.");
                }

                var courseSkills = await _coursesSkillRepository.GetCoursesSkillsByCourseIdAsync(oldCourse.Id);

                if (courseSkills is null ||
                    courseSkills.Count == 0)
                {
                    return Error.Validation(description: "Course has no skills.");
                }

                List<CoursesSkillsIdsDto> coursesSkillsResponse = [];

                foreach (CoursesSkills oldCourseSkill in courseSkills)
                {
                    var newCourseSkill = oldCourseSkill.UpdateCourseSkill(
                        courseId: course != null ? course.Id : oldCourseSkill.CourseId,
                        skillId: oldCourseSkill.SkillId,
                        weight: command.weight ?? oldCourseSkill.Weight);

                    if (newCourseSkill.IsError)
                    {
                        return newCourseSkill.Errors;
                    }

                    coursesSkillsResponse.Add(new CoursesSkillsIdsDto
                    {
                        CourseSkillId = newCourseSkill.Value.Id,
                        CourseId = newCourseSkill.Value.CourseId,
                        SkillId = newCourseSkill.Value.SkillId,
                        Weight = newCourseSkill.Value.Weight,

                        AffectedId = newCourseSkill.Value.Id
                    });

                    _coursesSkillRepository.UpdateCoursesSkills(oldCourseSkill, newCourseSkill.Value);
                }

                await _unitOfWork.CommitChangesAsync();

                return coursesSkillsResponse;
*/

/*
                ////// FOR THE IF CHECK OF DUPLICATE COURSE SKILLS BECAUSE OF THE UPDATE //////
                          ////// GIVES BETTER PERFORMANCE FROM O(n^2) TO O(n) //////
        var newSkillIds = new HashSet<int>(skillAndCourses.CoursesSkill.Select(cs => cs.SkillId));
        bool anyOverlap = oldSkillAndCourses.CoursesSkill.Any(oCS => newSkillIds.Contains(oCS.SkillId));
*/