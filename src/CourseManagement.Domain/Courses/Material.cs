using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Courses
{
    public class Material
    {
        public int Id { get; set; }
        public string Path { get; set; } = string.Empty;
        public bool IsVideo { get; set; }
        public int SessionId { get; set; }

        private Material(
            int sessionId,
            string path,
            bool isVideo)
        {
            SessionId = sessionId;
            Path = path;
            IsVideo = isVideo;
        }

        private ErrorOr<Success> CheckIfMaterialIsValid()
        {
            if (Path.Length < 0 || Path.Length > 500)
            {
                return MaterialErrors.PathIsTooLong;
            }

            if (SessionId <= 0)
            {
                return MaterialErrors.SessionIdIsAbnormal;
            }

            return Result.Success;
        }

        private static ErrorOr<Success> CheckIfSessionIdIsValid(int sessionId)
        {
            if (sessionId <= 0)
            {
                return MaterialErrors.SessionIdIsAbnormal;
            }

            return Result.Success;
        }

        private static ErrorOr<Success> CheckIfPathIsValid(string path)
        {
            if (path.Length < 0 || path.Length > 500)
            {
                return MaterialErrors.PathIsTooLong;
            }

            return Result.Success;
        }

        internal static ErrorOr<Material> CreateMaterial(
            int sessionId,
            string path,
            bool isVideo)
        {
            Material material = new Material(
                sessionId: sessionId,
                path: path,
                isVideo: isVideo);

            if (material.CheckIfMaterialIsValid() != Result.Success)
            {
                return material.CheckIfMaterialIsValid().Errors;
            }

            return material;
        }

        internal ErrorOr<Material> UpdateMaterial(
            string path,
            bool isVideo)
        {
            var materialValidity = CheckIfPathIsValid(path);

            if (materialValidity != Result.Success)
            {
                return materialValidity.Errors;
            }

            Path = path;
            IsVideo = isVideo;

            return this;
        }

        internal ErrorOr<Material> UpdateMaterialSessionPlacement(int sessionId)
        {
            var materialValidity = CheckIfSessionIdIsValid(sessionId);

            if (materialValidity != Result.Success)
            {
                return materialValidity.Errors;
            }

            SessionId = sessionId;

            return this;
        }
    }
}
