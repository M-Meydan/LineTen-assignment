using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Products.Update
{
    public class UpdateProductCommand : IRequest<Product>, IMapTo<Product>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int SKU { get; set; }


        public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Product>
        {
            readonly IProductRepository _productRepository;
            readonly IMapper _mapper;
            public UpdateProductHandler(IProductRepository customerRepository, IMapper mapper)
            {
                _productRepository = customerRepository;
                _mapper = mapper;
            }

            public async Task<Product> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
            {
                var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

                if (product != null)
                {
                    var updatedProduct = _mapper.Map(request, product);
                    await _productRepository.UpdateAsync(updatedProduct,cancellationToken);
                }

                return product;
            }
        }
    }
}
