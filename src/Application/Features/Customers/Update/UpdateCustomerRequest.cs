using Application.Common.Mappings;
using Domain;

namespace Application.Features.Customers.Update
{
    public class UpdateCustomerRequest : IMapTo<UpdateCustomerCommand>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; }
    }
}
