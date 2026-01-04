using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using ErrorOr;
using MediatR;
using CourseManagement.Application.Enrollments.Common.Dto;

namespace CourseManagement.Application.Enrollments.Queries.GetEnrollmentsToConfirm
{
    public class GetEnrollmentsToConfirmQueryHandler(
        IEnrollmentsRepository enrollmentsRepository
        ) : IRequestHandler<GetEnrollmentsToConfirmQuery, ErrorOr<List<EnrollmentDto>>>
    {
        private readonly IEnrollmentsRepository _enrollmentsRepository = enrollmentsRepository;

        public async Task<ErrorOr<List<EnrollmentDto>>> Handle(GetEnrollmentsToConfirmQuery query, CancellationToken cancellationToken)
        {
            try
            {
                return await _enrollmentsRepository.GetAllEnrollmentsToBeConfirmed();
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
