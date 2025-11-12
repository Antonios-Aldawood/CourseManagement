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
            string path,
            bool isVideo)
        {
            Path = path;
            IsVideo = isVideo;
        }

        private ErrorOr<Success> ValidMaterial()
        {
            if (SessionId <= 0)
            {
                return MaterialErrors.SessionIdIsAbnormal;
            }

            return Result.Success;
        }

        internal static ErrorOr<Material> CreateMaterial(
            string path,
            bool isVideo)
        {
            Material material = new Material(
                path,
                isVideo);

            if (material.ValidMaterial() != Result.Success)
            {
                return material.ValidMaterial().Errors;
            }

            return material;
        }
    }
}
