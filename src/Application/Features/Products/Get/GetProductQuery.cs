using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Products.Common;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Application.Features.Products.Get
{
    public class GetProductQuery : IRequest<ProductResponse>
    {
        /// <summary>
        /// Id
        /// </summary>
        /// <example>GUID</example>
        public Guid Id { get; set; }

        public GetProductQuery() { }
        public GetProductQuery(Guid id)
        {
            Id = id;
        }

        public class GetProductHandler : IRequestHandler<GetProductQuery, ProductResponse>
        {
            readonly IProductRepository _productRepository;
            readonly IValidator<GetProductQuery> _validator;
            readonly IMapper _mapper;

            public GetProductHandler(IProductRepository customerRepository,
                IValidator<GetProductQuery> validator,
                IMapper mapper)
            {
                _productRepository = customerRepository;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<ProductResponse> Handle(GetProductQuery query, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(query);

                if (!validationResult.IsValid)
                    throw new ValidationException(validationResult.Errors);

                var product = await _productRepository.GetByIdAsync(query.Id);
                return _mapper.Map<ProductResponse>(product);
            }
        }
    }
}
