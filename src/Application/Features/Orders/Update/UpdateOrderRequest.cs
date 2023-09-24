using Application.Common.Mappings;
using Domain;
using Domain.Enums;

namespace Application.Features.Orders.Update
{
    public class UpdateOrderRequest : IMapTo<UpdateOrderCommand>
    {
        public OrderStatus Status { get; set; }
    }
}
