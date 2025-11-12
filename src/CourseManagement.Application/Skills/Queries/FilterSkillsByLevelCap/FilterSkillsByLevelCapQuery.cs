using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Application.Skills.Common.Dto;


namespace CourseManagement.Application.Skills.Queries.FilterSkillsByLevelCap
{
    public record FilterSkillsByLevelCapQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        int levelCap) : IRequest<ErrorOr<List<SkillSoleDto>>>, IHeaderCarrier;
}
