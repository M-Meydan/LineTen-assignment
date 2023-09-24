using Application.Common.Mappings;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.Orders.Common
{
    public class OrderResponse : IMapFrom<Order>
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
