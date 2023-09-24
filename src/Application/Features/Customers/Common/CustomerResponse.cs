using Application.Common.Mappings;
using Domain.Entities;

namespace Application.Features.Customers.Common
{
    public class CustomerResponse : IMapFrom<Customer>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; }
    }
}
