using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Common.Mappings;
using Application.Common.Models;
using Application.Features.Products.Common;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace Application.Features.Products.GetList
{
    public class GetProductListQuery : IRequest<PaginatedList<ProductResponse>>
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

        public GetProductListQuery() { }

        public class GetProductListQueryHandler : IRequestHandler<GetProductListQuery, PaginatedList<ProductResponse>>
        {
            readonly IProductRepository _productRepository;
            readonly IMapper _mapper;

            public GetProductListQueryHandler(IProductRepository customerRepository, IMapper mapper)
            {
                _productRepository = customerRepository;
                _mapper = mapper;
            }

            public Task<PaginatedList<ProductResponse>> Handle(GetProductListQuery request, CancellationToken cancellationToken)
            {
                var result = _productRepository.GetQueryable()
                 .ProjectTo<ProductResponse>(_mapper.ConfigurationProvider)
                 .PaginatedList(request.PageNumber, request.PageSize);

                return Task.FromResult(result);
            }
        }

    }

}
