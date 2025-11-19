using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;

namespace CourseManagement.Application.Courses.Queries.GetCoursesSessionsMaterials
{
    public class GetCoursesSessionsMaterialsQueryHandler(
        ICoursesRepository coursesRepository
        ) : IRequestHandler<GetCoursesSessionsMaterialsQuery, ErrorOr<List<CourseWithSessionsAndMaterialsDto>>>
    {
        private readonly ICoursesRepository _coursesRepository = coursesRepository;

        public async Task<ErrorOr<List<CourseWithSessionsAndMaterialsDto>>> Handle(GetCoursesSessionsMaterialsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var courses = await _coursesRepository.GetCoursesWithSessionsAndSessionsMaterials();
                
                return courses
                    .Select(CourseWithSessionsAndMaterialsDto.AddDto)
                    .ToList();
            }
            catch(Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
