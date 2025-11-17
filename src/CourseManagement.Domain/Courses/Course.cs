using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;
using CourseManagement.Domain.Users;

namespace CourseManagement.Domain.Courses
{
    public class Course
    {
        public int Id { get; init; }
        public string Subject { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public List<Eligibility>? Eligibilities { get; init; } = [];
        public List<int>? CoursesSkillsIds { get; private set; } = [];
        public bool IsForAll { get; set; }
        public List<Session>? Sessions { get; set; } = [];

        private Course(
            string subject,
            string description)
        {
            Subject = subject;
            Description = description;
        }

        private ErrorOr<Success> CheckIfCourseIsValid()
        {
            if (Subject.Length < 3 || Subject.Length > 35)
            {
                return CourseErrors.CourseSubjectIsNotValid;
            }

            return Result.Success;
        }

        private static ErrorOr<Success> CheckIfCourseSubjectIsValid(string subject)
        {
            if (subject.Length < 3 || subject.Length > 35)
            {
                return CourseErrors.CourseSubjectIsNotValid;
            }

            return Result.Success;
        }

        public Success SpecifyCourseEligibilityForAll()
        {
            IsForAll = true;

            if (Eligibilities != null)
            {
                Eligibilities.RemoveAll(e => e is not null);
            }

            return Result.Success;
        }

        public ErrorOr<Eligibility> SpecifyCourseEligibility(
            string key,
            int value)
        {
            if (Eligibilities is not null)
            {
                foreach (Eligibility eligibility in Eligibilities)
                {
                    if (eligibility.Key == key &&
                        eligibility.Value == value)
                    {
                        return CourseErrors.EligibilityAlreadyGivenToCourse;
                    }
                }
            }

            string[] userPositions = User.Positions();

            var newEligibility = Eligibility.CreateEligibility(
                courseId: Id,
                key: key,
                value: value,
                positions: userPositions);

            if (newEligibility.IsError)
            {
                return newEligibility.Errors;
            }

            Eligibilities!.Add(newEligibility.Value);

            return newEligibility;
        }

        public ErrorOr<Session> AddCourseSession(
            string sessionName,
            DateTimeOffset startDate,
            DateTimeOffset endDate,
            int trainerId,
            bool isOffline,
            int? seats,
            string? link,
            string? app)
        {
            if (Sessions is not null)
            {
                foreach (Session session in Sessions)
                {
                    if (
                        session.EndDate.CompareTo(DateTimeOffset.UtcNow) >= 0 &&
                        !(
                            (startDate.CompareTo(session.StartDate) <= 0 && endDate.CompareTo(session.StartDate) <= 0)
                                ||
                            (startDate.CompareTo(session.EndDate) >= 0 && endDate.CompareTo(session.EndDate) >= 0)
                         )
                       )
                    {
                        return CourseErrors.SessionTimeFrameConflictsWithOtherSessions;
                    }

                    if (session.Name == sessionName ||
                        (session.StartDate.CompareTo(startDate) == 0 && session.EndDate.CompareTo(endDate) == 0))
                    {
                        return CourseErrors.SessionAlreadyAssignedToCourse;
                    }
                }
            }

            var newSession = Session.CreateSession(
                name: sessionName,
                courseId: Id,
                startDate: startDate,
                endDate: endDate,
                trainerId: trainerId,
                isOffline: isOffline,
                seats: seats,
                link: link,
                app: app);

            if (newSession.IsError)
            {
                return newSession.Errors;
            }

            Sessions!.Add(newSession.Value);

            return newSession.Value;
        }

        public ErrorOr<Material> AddCourseSessionMaterial(
            int sessionId,
            string path,
            bool isVideo)
        {
                    ///////// Before it got needed in other methods. /////////
            /*
            if (Sessions!.FirstOrDefault(s => s.Id == sessionId) is not Session session)
            {
                return CourseErrors.SessionNotFound;
            }
            */

            var session = CheckIfCourseHasSessionBySessionId(sessionId);
            
            if (session.IsError)
            {
                return session.Errors;
            }

            var material = session.Value.AddSessionMaterial(
                path: path,
                isVideo: isVideo);

            if (material.IsError)
            {
                return material.Errors;
            }

            return material.Value;
        }

        public ErrorOr<Material> UpdateCourseSessionMaterial(
            int sessionId,
            int materialId,
            string path,
            bool isVideo)
        {
            var session = CheckIfCourseHasSessionBySessionId(sessionId);

            if (session.IsError)
            {
                return session.Errors;
            }

            var updatedMaterial = session.Value.UpdateSessionMaterial(
                materialId: materialId,
                path: path,
                isVideo: isVideo);

            if (updatedMaterial.IsError)
            {
                return updatedMaterial.Errors;
            }

            return updatedMaterial.Value;
        }

        public ErrorOr<Material> UpdateCourseSessionMaterialSessionPlacement(
            int oldMaterialId,
            int oldSessionId,
            string materialNewSessionName,
            Course? newCourse)
        {
            var oldSession = CheckIfCourseHasSessionBySessionId(oldSessionId);

            if (oldSession.IsError)
            {
                return oldSession.Errors;
            }

            ErrorOr<Session> newSession;

            if (newCourse != null)
            {
                newSession = newCourse.CheckIfCourseHasSession(materialNewSessionName);

                if (newSession.IsError)
                {
                    return newSession.Errors;
                }
            }
            else
            {
                newSession = CheckIfCourseHasSession(materialNewSessionName);

                if (newSession.IsError)
                {
                    return newSession.Errors;
                }
            }
            
    ///////// With it, we can't update material for sessions outside this course /////////
            /*
            var newSessionExists = CheckIfCourseHasSessionBySessionId(newSession.Id);

            if (newSessionExists.IsError)
            {
                return newSessionExists.Errors;
            }
            */

            var updatedMaterial = oldSession.Value.UpdateSessionMaterialSessionPlacement(
                oldMaterialId: oldMaterialId,
                newSession: newSession.Value);

            if (updatedMaterial.IsError)
            {
                return updatedMaterial.Errors;
            }

            return updatedMaterial.Value;
        }

        public ErrorOr<Session> CheckIfCourseHasSession(string sessionName)
        {
            if (Sessions is not null &&
                Sessions.Count != 0 &&
                Sessions.FirstOrDefault(s => s.Name == sessionName) is Session session)
            {
                return session;
            }

            return CourseErrors.SessionNotFound;
        }

        public ErrorOr<Session> CheckIfCourseHasSessionBySessionId(int sessionId)
        {
            if (Sessions is not null &&
                Sessions.Count != 0 &&
                Sessions.FirstOrDefault(s => s.Id == sessionId) is Session session)
            {
                return session;
            }

            return CourseErrors.SessionNotFound;
        }

        public static ErrorOr<Course> CreateCourse(
            string subject,
            string description)
        {
            Course course = new Course(
                subject: subject,
                description: description);

            var courseValidity = course.CheckIfCourseIsValid();
            
            if (courseValidity != Result.Success)
            {
                return courseValidity.Errors;
            }

            return course;
        }

        public ErrorOr<Course> UpdateCourse(
            string subject,
            string description)
        {
            var courseValidity = CheckIfCourseSubjectIsValid(subject);

            if (courseValidity != Result.Success)
            {
                return courseValidity.Errors;
            }

            Subject = subject;
            Description = description;

            return this;
        }
    }
}

/*
                ///////// PROBABLY BETTER UPDATING METHOD, IN RESPECT TO DDD /////////
public ErrorOr<Skill> UpdateSkill(
            string oldSkillName,
            string skillName,
            int levelCap)
        {
            if (Skills is not null)
            {
                foreach (Skill skill in Skills)
                {
                    if (skill.SkillName == oldSkillName)
                    {
                        Skill oldSkill = skill;

                        var newSkill = Skill.UpdateSkill(
                            oldSkill: oldSkill,
                            skillName: skillName,
                            levelCap: levelCap);

                        if (newSkill.IsError)
                        {
                            return newSkill.Errors;
                        }

                        return newSkill;
                    }

                    return CourseErrors.CourseDoesNotHaveSkill;
                }
            }

            return CourseErrors.CourseDoesNotHaveSkill;
        } 
*/


/*
        public List<Skill>? Skills { get; set; } = [];
        public List<CoursesSkills>? CoursesSkillsOwned { get; set; } = [];

        public void SetSkills(List<Skill>? skills)
        {
            Skills = skills;
        }

        public ErrorOr<Skill> AddSkill(
            string skillName,
            int levelCap)
        {
            if (Skills is not null &&
                Skills.Count > 0)
            {
                foreach (Skill existingSkill in Skills)
                {
                    if (existingSkill.SkillName == skillName)
                    {
                        return CourseErrors.SkillAlreadyExisting;
                    }
                }
            }

            var skill = Skill.CreateSkill(
                skillName,
                levelCap);

            if (skill.IsError)
            {
                return skill.Errors;
            }

            if (Skills is not null)
            {
                Skills.Add(skill.Value);
            }

            return skill;
        }

        public ErrorOr<Skill> UpdateSkill(
            Skill oldSkill,
            string skillName,
            int levelCap)
        {
            if (Skills is null ||
                Skills.Contains(oldSkill) == false)
            {
                return CourseErrors.CourseDoesNotHaveSkill;
            }

            var newSkill = Skill.UpdateSkill(
                oldSkill: oldSkill,
                skillName: skillName,
                levelCap: levelCap);

            if (newSkill.IsError)
            {
                return newSkill.Errors;
            }

            return newSkill.Value;
        }

        public ErrorOr<CoursesSkills> AddCourseSkill(
            int skillId,
            int weight)
        {
            if (CoursesSkillsOwned is not null &&
                CoursesSkillsOwned.Count > 0)
            {
                foreach (CoursesSkills existingCourseSkill in CoursesSkillsOwned)
                {
                    if (existingCourseSkill.CourseId == Id &&
                        existingCourseSkill.SkillId == skillId)
                    {
                        return CourseErrors.CourseAlreadyHasSkill;
                    }
                }
            }

            var courseSkill = CoursesSkills.CreateCourseSkill(
                courseId: Id,
                skillId: skillId,
                weight: weight);

            if (courseSkill.IsError)
            {
                return courseSkill.Errors;
            }

            if (CoursesSkillsOwned is not null)
            {
                CoursesSkillsOwned.Add(courseSkill.Value);
            }

            return courseSkill;
        }

        public ErrorOr<CoursesSkills> UpdateCourseSkill(
            CoursesSkills oldCourseSkill,
            int skillId,
            int weight)
        {
            if (CoursesSkillsOwned is not null &&
                CoursesSkillsOwned.Contains(oldCourseSkill) == false)
            {
                return CourseErrors.CourseDoesNotHaveSkill;
            }

            var courseSkill = CoursesSkills.UpdateCourseSkill(
                oldCourseSkill: oldCourseSkill,
                courseId: Id,
                skillId: skillId,
                weight: weight);

            if (courseSkill.IsError)
            {
                return courseSkill.Errors;
            }

            return courseSkill.Value;
        }

        /////////// Was put in the start of AddCourseSession for checking session times conflict ///////////
        if ((session.StartDate.CompareTo(startDate) >= 0 && session.EndDate.CompareTo(endDate) <= 0) ||
            (session.StartDate.CompareTo(startDate) >= 0 && session.EndDate.CompareTo(endDate) >= 0) ||
            (session.StartDate.CompareTo(startDate) <= 0 && session.EndDate.CompareTo(endDate) <= 0))
            {
                return CourseErrors.SessionTimeFrameConflictsWithOtherSessions;
            }
*/