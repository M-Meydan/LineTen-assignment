using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Common.Mappings;
using Application.Features.Products.Common;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.Products.Create
{
    public class CreateProductCommand : IRequest<ProductResponse>, IMapTo<Product>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int SKU { get; set; }


        public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponse>
        {
            readonly IProductRepository _productRepository;
            readonly IValidator<CreateProductCommand> _validator;
            readonly IMapper _mapper;

            public CreateProductHandler(IProductRepository customerRepository,
                IValidator<CreateProductCommand> validator,
                IMapper mapper,
                IMediator mediator)
            {
                _productRepository = customerRepository;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<ProductResponse> Handle(CreateProductCommand command, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(command);

                if (!validationResult.IsValid)
                    throw new ValidationException(validationResult.Errors);

                var entity = _mapper.Map<Product>(command);

                var product = await _productRepository.AddAsync(entity, cancellationToken);

                return _mapper.Map<ProductResponse>(product);
            }
        }
    }
}
