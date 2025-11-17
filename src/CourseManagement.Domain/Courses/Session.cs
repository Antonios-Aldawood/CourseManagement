using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Courses
{
    public class Session
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public int TrainerId { get; set; }
        public bool IsOffline { get; set; }
        public int? Seats { get; set; }
        public string? Link { get; set; } = string.Empty;
        public string? App { get; set; } = string.Empty;
        public List<Material>? Materials { get; set; } = [];

        private Session(
            int courseId,
            string name,
            DateTimeOffset startDate,
            DateTimeOffset endDate,
            int trainerId,
            bool isOffline,
            int? seats,
            string? link,
            string? app)
        {
            Name = name;
            CourseId = courseId;
            StartDate = startDate;
            EndDate = endDate;
            TrainerId = trainerId;
            IsOffline = isOffline;
            Seats = seats;
            Link = link;
            App = app;
        }

        private static ErrorOr<Success> ValidateStartAndEndDate(
            DateTimeOffset startDate,
            DateTimeOffset endDate)
        {
            if (startDate >= DateTimeOffset.UtcNow.AddMonths(1) ||
                endDate >= DateTimeOffset.UtcNow.AddMonths(3))
            {
                return SessionErrors.StartTimeOrEndTimeBeyondBoundaries;
            }

            if (startDate >= endDate)
            {
                return SessionErrors.StartTimeCannotBeAfterEndTime;
            }

            return Result.Success;
        }

        private ErrorOr<Success> ValidateSession()
        {
            if (StartDate.CompareTo(DateTimeOffset.UtcNow) < 0 ||
                EndDate.CompareTo(DateTimeOffset.UtcNow) < 0 ||
                StartDate.CompareTo(DateTimeOffset.UtcNow.AddMonths(3)) >= 0 ||
                EndDate.CompareTo(DateTimeOffset.UtcNow.AddMonths(3)) >= 0)
            {
                return SessionErrors.StartTimeOrEndTimeBeyondBoundaries;
            }

            if (StartDate.CompareTo(EndDate) >= 0)
            {
                return SessionErrors.StartTimeCannotBeAfterEndTime;
            }

            if (EndDate.CompareTo(StartDate.AddHours(10)) >= 0)
            {
                return SessionErrors.EndTimeCannotBeMoreThan10HoursAfterStartTime;
            }

            if (Seats >= 50)
            {
                return SessionErrors.SeatsExceedingLimit;
            }

            return Result.Success;
        }

        internal ErrorOr<Material> AddSessionMaterial(
            string path,
            bool isVideo)
        {
            if (Materials is not null)
            {
                if (Materials.Count >= 4)
                {
                    return SessionErrors.SessionCanNotHaveMoreThanFourMaterials;
                }

                foreach (Material material in Materials)
                {
                    if (material.Path == path &&
                        material.IsVideo == isVideo)
                    {
                        return SessionErrors.MaterialAlreadyGivenToSession;
                    }
                }
            }

            var newMaterial = Material.CreateMaterial(
                sessionId: Id,
                path: path,
                isVideo: isVideo);

            if (newMaterial.IsError)
            {
                return newMaterial.Errors;
            }

            Materials!.Add(newMaterial.Value);

            return newMaterial;
        }

        internal ErrorOr<Material> UpdateSessionMaterial(
            int materialId,
            string path,
            bool isVideo)
        {
            Material? oldMaterial = Materials?.FirstOrDefault(m => m.Id == materialId);

            if (oldMaterial == null)
            {
                return SessionErrors.SessionMaterialNotFound;
            }

            if (Materials is not null)
            {
                foreach (Material material in Materials)
                {
                    if (material.Path == path &&
                        material.IsVideo == isVideo)
                    {
                        return SessionErrors.MaterialAlreadyGivenToSession;
                    }
                }
            }

            var updatedMaterial = oldMaterial.UpdateMaterial(
                path: path,
                isVideo: isVideo);

            if (updatedMaterial.IsError)
            {
                return updatedMaterial.Errors;
            }

            return updatedMaterial;
        }

        internal ErrorOr<Material> UpdateSessionMaterialSessionPlacement(
            int oldMaterialId,
            Session newSession)
        {
            Material? oldMaterial = Materials?.FirstOrDefault(m => m.Id == oldMaterialId);

            if (oldMaterial == null)
            {
                return SessionErrors.SessionMaterialNotFound;
            }

            if (newSession.Materials != null)
            {
                if (newSession.Materials.Count >= 4)
                {
                    return SessionErrors.SessionCanNotHaveMoreThanFourMaterials;
                }

                foreach (Material material in newSession.Materials)
                {
                    if (material.Path == oldMaterial.Path &&
                        material.IsVideo == oldMaterial.IsVideo)
                    {
                        return SessionErrors.MaterialAlreadyGivenToSession;
                    }
                }
            }

            var updatedMaterial = oldMaterial.UpdateMaterialSessionPlacement(newSession.Id);

            if (updatedMaterial.IsError)
            {
                return updatedMaterial.Errors;
            }

            newSession.Materials!.Add(updatedMaterial.Value);
            
            Materials!.Remove(oldMaterial);

            return updatedMaterial;
        }

        internal static ErrorOr<Session> CreateSession(
            string name,
            int courseId,
            DateTimeOffset startDate,
            DateTimeOffset endDate,
            int trainerId,
            bool isOffline,
            int? seats,
            string? link,
            string? app)
        {
            Session session = new Session(
                name: name,
                courseId: courseId,
                startDate: startDate,
                endDate: endDate,
                trainerId: trainerId,
                isOffline: isOffline,
                seats: seats,
                link: link,
                app: app);

            if (session.ValidateSession() != Result.Success)
            {
                return session.ValidateSession().Errors;
            }

            return session;
        }
    }
}

/*
        private ErrorOr<Success> CheckIfMaterialIsDuplicatedOrMoreThanFour(
            string path,
            bool isVideo)
        {
            if (Materials is not null)
            {
                if (Materials.Count >= 4)
                {
                    return SessionErrors.SessionCanNotHaveMoreThanFourMaterials;
                }

                foreach (Material material in Materials)
                {
                    if (material.Path == path &&
                        material.IsVideo == isVideo)
                    {
                        return SessionErrors.MaterialAlreadyGivenToSession;
                    }
                }
            }

            return Result.Success;
        }
*/