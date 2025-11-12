using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Users
{
    public class Address
    {
        public int Id { get; init; }
        public string City { get; init; } = string.Empty;
        public string Region { get; init; } = string.Empty;
        public string Road { get; init; } = string.Empty;

        internal Address(
            string city,
            string region,
            string road)
        {
            City = city;
            Region = region;
            Road = road;
        }
    }
}
