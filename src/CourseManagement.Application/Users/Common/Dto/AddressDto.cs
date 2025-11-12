using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.Users;

namespace CourseManagement.Application.Users.Common.Dto
{
    public record AddressDto : IHasAffectedIds
    {
        public required int AddressId { get; set; }
        public required string City { get; set; }
        public required string Region { get; set; }
        public required string Road { get; set; }

        public int AffectedId { get; set; }

        public static AddressDto AddDto(Address address)
        {
            AddressDto dto = new AddressDto
            {
                AddressId = address.Id,
                City = address.City,
                Region = address.Region,
                Road = address.Road,
            };

            dto.AffectedId = address.Id;

            return dto;
        }
    }
}
