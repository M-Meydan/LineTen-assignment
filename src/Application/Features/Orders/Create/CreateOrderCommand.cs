using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Common.Mappings;
using Application.Features.Orders.Common;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;

namespace Application.Features.Orders.Create
{
    public class CreateOrderCommand : IRequest<OrderResponse>, IMapTo<Order>
    {
        public Guid ProductId { get; set; }
        public Guid CustomerId { get; set; }

        public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IValidator<CreateOrderCommand> _validator;
            private readonly IMapper _mapper;

            public CreateOrderHandler(IOrderRepository customerRepository,
                IValidator<CreateOrderCommand> validator,
                IMapper mapper,
                IMediator mediator)
            {
                _orderRepository = customerRepository;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<OrderResponse> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(command);

                if (!validationResult.IsValid)
                    throw new ValidationException(validationResult.Errors);

                var entity = _mapper.Map<Order>(command);

                var order = await _orderRepository.AddAsync(entity, cancellationToken);

                return _mapper.Map<OrderResponse>(order);
            }
        }
    }
}
