using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.CourseSkill.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.CourseSkill.Queries.GetCourseSkillCourses
{
    public record GetCourseSkillCoursesQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        int skillId) : IRequest<ErrorOr<List<CoursesSkillsDto>>>, IHeaderCarrier;
}
