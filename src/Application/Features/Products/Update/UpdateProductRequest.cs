using Application.Common.Mappings;
using Application.Features.Customers.Update;
using Domain;

namespace Application.Features.Products.Update
{
    public class UpdateProductRequest : IMapTo<UpdateCustomerCommand>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
