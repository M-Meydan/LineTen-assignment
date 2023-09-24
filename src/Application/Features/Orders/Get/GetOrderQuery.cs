using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Orders.Common;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Application.Features.Orders.Get
{
    public class GetOrderQuery : IRequest<OrderResponse>
    {
        /// <summary>
        /// Id
        /// </summary>
        /// <example>GUID</example>
        public Guid Id { get; set; }

        public GetOrderQuery() { }
        public GetOrderQuery(Guid id)
        {
            Id = id;
        }

        public class GetOrderHandler : IRequestHandler<GetOrderQuery, OrderResponse>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IValidator<GetOrderQuery> _validator;
            private readonly IMapper _mapper;

            public GetOrderHandler(IOrderRepository orderRepository,
                IValidator<GetOrderQuery> validator,
                IMapper mapper)
            {
                _orderRepository = orderRepository;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<OrderResponse> Handle(GetOrderQuery query, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(query);

                if (!validationResult.IsValid)
                    throw new ValidationException(validationResult.Errors);

                var order = await _orderRepository.GetByIdAsync(query.Id);
                return _mapper.Map<OrderResponse>(order);
            }
        }
    }
}
