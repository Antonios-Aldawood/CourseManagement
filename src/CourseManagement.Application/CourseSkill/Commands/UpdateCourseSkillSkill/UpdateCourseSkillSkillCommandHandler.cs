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

namespace CourseManagement.Application.CourseSkill.Commands.UpdateCourseSkillSkill
{
    public class UpdateCourseSkillSkillCommandHandler(
        ICoursesSkillsRepository coursesSkillsRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateCourseSkillSkillCommand, ErrorOr<List<CoursesSkillsIdsDto>>>
    {
        private readonly ICoursesSkillsRepository _coursesSkillsRepository = coursesSkillsRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<List<CoursesSkillsIdsDto>>> Handle(UpdateCourseSkillSkillCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var oldSkillAndSkillWithCourses = await _coursesSkillsRepository.GetOldSkillAndSkillWithSkillCoursesByIdAsync(command.oldSkillId, command.skillId ?? 0);

                var oldSkillAndCourses = oldSkillAndSkillWithCourses
                    .Where(oSASWC => oSASWC.SkillId == command.oldSkillId)
                    .FirstOrDefault();

                if (oldSkillAndCourses == null ||
                    oldSkillAndCourses.SkillId == null)
                {
                    return Error.Validation(description: "Skill does not exist.");
                }

                if (oldSkillAndCourses.CoursesSkill == null ||
                    oldSkillAndCourses.CoursesSkill.Count == 0)
                {
                    return Error.Validation(description: "Skill was given to no courses.");
                }

                var skillAndCourses = oldSkillAndSkillWithCourses
                    .Where(oSASWC => oSASWC.SkillId == command.skillId)
                    .FirstOrDefault();

                if (command.skillId != null &&
                    (skillAndCourses == null || skillAndCourses.SkillId == null))
                {
                    return Error.Validation(description: "Target skill not found.");
                }

                if (command.skillId != null &&
                    skillAndCourses!.CoursesSkill != null &&
                    skillAndCourses!.CoursesSkill.Count != 0)
                {
                    var courseIds = new HashSet<int>(skillAndCourses!.CoursesSkill.Select(cs => cs.CourseId));
                    bool duplicates = oldSkillAndCourses.CoursesSkill.Any(oCS => courseIds.Contains(oCS.CourseId));
                    
                    if (duplicates == true)
                    {
                        return Error.Validation(description: "Updating course skill will cause duplicates.");
                    }
                }

                List<CoursesSkillsIdsDto> coursesSkillsResponse = [];

                foreach (CoursesSkills oldCourseSkill in oldSkillAndCourses.CoursesSkill)
                {
                    var newCourseSkill = oldCourseSkill.UpdateCourseSkill(
                        courseId: oldCourseSkill.CourseId,
                        //skillId: skillAndCourses != null && skillAndCourses.SkillId != null ? skillAndCourses.SkillId ?? 0 : oldCourseSkill.SkillId,
                        skillId: command.skillId != null ? skillAndCourses!.SkillId ?? 0 : oldCourseSkill.SkillId,
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

                    _coursesSkillsRepository.UpdateCoursesSkills(oldCourseSkill, newCourseSkill.Value);
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
            ////////// WORKING INSIDE TRY //////////
        ////////// ADD COURSES REPOSITORY FOR THIS TO WORK //////////
        
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

                var skillCourses = await _coursesSkillsRepository.GetCoursesSkillsBySkillIdAsync(oldSkill.Id);

                if (skillCourses is null ||
                    skillCourses.Count == 0)
                {
                    return Error.Validation(description: "Skill was given to no courses.");
                }

                List<CoursesSkillsIdsDto> coursesSkillsResponse = [];

                foreach (CoursesSkills oldCourseSkill in skillCourses)
                {
                    var newCourseSkill = oldCourseSkill.UpdateCourseSkill(
                        courseId: oldCourseSkill.CourseId,
                        skillId: skill != null ? skill.Id : oldCourseSkill.SkillId,
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

                    _coursesSkillsRepository.UpdateCoursesSkills(oldCourseSkill, newCourseSkill.Value);
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