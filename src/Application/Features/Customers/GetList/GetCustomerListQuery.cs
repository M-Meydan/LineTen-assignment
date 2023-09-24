using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Common.Mappings;
using Application.Common.Models;
using Application.Features.Customers.Common;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace Application.Features.Customers.GetList
{
    public class GetCustomerListQuery : IRequest<PaginatedList<CustomerResponse>>
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

        public GetCustomerListQuery() { }

        public class GetCustomerListQueryHandler : IRequestHandler<GetCustomerListQuery, PaginatedList<CustomerResponse>>
        {
            readonly ICustomerRepository _customerRepository;
            readonly IMapper _mapper;

            public GetCustomerListQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
            {
                _customerRepository = customerRepository;
                _mapper = mapper;
            }

            public Task<PaginatedList<CustomerResponse>> Handle(GetCustomerListQuery request, CancellationToken cancellationToken)
            {
                var result = _customerRepository.GetQueryable()
                 .ProjectTo<CustomerResponse>(_mapper.ConfigurationProvider)
                 .PaginatedList(request.PageNumber, request.PageSize);

                return Task.FromResult(result);
            }
        }

    }

}
