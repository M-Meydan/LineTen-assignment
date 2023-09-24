using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.Orders.Delete
{
    public class DeleteOrderCommand : IRequest<Order>
    {
        public Guid Id { get; set; }

        public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, Order>
        {
            private readonly IOrderRepository _orderRepository;

            public DeleteOrderHandler(IOrderRepository orderRepository)
            {
                _orderRepository = orderRepository;
            }

            public async Task<Order> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
            {
                var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);

                if (order != null)
                    await _orderRepository.DeleteAsync(order);

                return order;
            }
        }
    }
}
