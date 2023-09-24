using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Orders.Update
{
    public class UpdateOrderCommand : IRequest<Order>, IMapTo<Order>
    {
        public Guid Id { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;


        public class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand, Order>
        {
            readonly IOrderRepository _orderRepository;
            readonly IMapper _mapper;
            public UpdateOrderHandler(IOrderRepository orderRepository, IMapper mapper)
            {
                _orderRepository = orderRepository;
                _mapper = mapper;
            }

            public async Task<Order> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
            {
                var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);

                if (order != null)
                {
                    var updatedOrder = _mapper.Map(request, order);
                    await _orderRepository.UpdateAsync(updatedOrder,cancellationToken);
                }

                return order;
            }
        }
    }
}
