using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Common.Mappings;
using Application.Common.Models;
using Application.Features.Orders.Common;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace Application.Features.Orders.GetList
{
    public class GetOrderListQuery : IRequest<PaginatedList<OrderResponse>>
    {
        /// <summary>
        /// Page number for pagination. Default = 1.
        /// </summary>
        /// <example>1</example>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Number of items per page for pagination. Default =10
        /// </summary>
        /// <example>10</example>
        public int PageSize { get; set; } = 10;

        public GetOrderListQuery() { }

        public class GetOrderListQueryHandler : IRequestHandler<GetOrderListQuery, PaginatedList<OrderResponse>>
        {
            readonly IOrderRepository _orderRepository;
            readonly IMapper _mapper;

            public GetOrderListQueryHandler(IOrderRepository customerRepository, IMapper mapper)
            {
                _orderRepository = customerRepository;
                _mapper = mapper;
            }

            public Task<PaginatedList<OrderResponse>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
            {
                var result = _orderRepository.GetQueryable()
                 .ProjectTo<OrderResponse>(_mapper.ConfigurationProvider)
                 .PaginatedList(request.PageNumber, request.PageSize);

                return Task.FromResult(result);
            }
        }

    }

}
