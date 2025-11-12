using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Users.Common.Dto;
using CourseManagement.Domain.Users;

namespace CourseManagement.Application.Users.Queries.GetAllAddresses
{
    public class GetAllAddressesQueryHandler(
        IUsersRepository usersRepository
        ) : IRequestHandler<GetAllAddressesQuery, ErrorOr<List<AddressDto>>>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;

        public async Task<ErrorOr<List<AddressDto>>> Handle(GetAllAddressesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var foundAddresses = await _usersRepository.GetAllAddressesAsync();

                if (foundAddresses is not List<Address> addresses)
                {
                    return Error.Validation(description: "No addresses found.");
                }

                List<AddressDto> addressResponse = [];

                foreach (Address address in foundAddresses)
                {
                    AddressDto dto = AddressDto.AddDto(address);

                    addressResponse.Add(dto);
                }

                return addressResponse;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
