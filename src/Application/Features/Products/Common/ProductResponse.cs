using Application.Common.Mappings;
using Domain.Entities;

namespace Application.Features.Products.Common
{
    public class ProductResponse : IMapFrom<Product>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SKU { get; set; }
    }
}
