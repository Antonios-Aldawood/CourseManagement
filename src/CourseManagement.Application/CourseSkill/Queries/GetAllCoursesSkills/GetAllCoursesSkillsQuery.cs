using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.CourseSkill.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.CourseSkill.Queries.GetAllCoursesSkills
{
    public record GetAllCoursesSkillsQuery(
        string ipAddress,
        Dictionary<string, string> headers) : IRequest<ErrorOr<List<CoursesSkillsDto>>>, IHeaderCarrier;
}
