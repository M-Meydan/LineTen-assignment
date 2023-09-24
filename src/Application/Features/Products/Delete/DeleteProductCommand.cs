using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.Products.Delete
{
    public class DeleteProductCommand : IRequest<Product>
    {
        public Guid Id { get; set; }

        public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Product>
        {
            private readonly IProductRepository _productRepository;

            public DeleteProductHandler(IProductRepository productRepository)
            {
                _productRepository = productRepository;
            }

            public async Task<Product> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
            {
                var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

                if (product != null)
                    await _productRepository.DeleteAsync(product);

                return product;
            }
        }
    }
}
