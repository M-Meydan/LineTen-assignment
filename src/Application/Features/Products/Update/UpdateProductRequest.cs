using Application.Common.Mappings;
using Domain;

namespace Application.Features.Customers.Update
{
    public class UpdateProductRequest : IMapTo<UpdateCustomerCommand>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
