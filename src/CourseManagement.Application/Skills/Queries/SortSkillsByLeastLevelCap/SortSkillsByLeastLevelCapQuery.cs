using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Skills.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Skills.Queries.SortSkillsByLeastLevelCap
{
    public record SortSkillsByLeastLevelCapQuery(
        string ipAddress,
        Dictionary<string, string> headers) : IRequest<ErrorOr<List<SkillSoleDto>>>, IHeaderCarrier;
}
