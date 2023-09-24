using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Domain.Enums;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Features.Orders.Create
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        readonly ICustomerRepository _customerRepository;
        readonly IProductRepository _productRepository;

        public CreateOrderCommandValidator(ICustomerRepository customerRepository, IProductRepository productRepository)
        {
            _customerRepository = customerRepository;
            _productRepository = productRepository;

            RuleFor(x => x.ProductId)
           .NotEmpty().WithMessage("Product ID is required.")
           .MustAsync((productId, cancellationToken) => ProductExists(productId, cancellationToken))
           .WithMessage("Product not found.");

            RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.")
            .MustAsync((customerId, cancellationToken) => CustomerExists(customerId, cancellationToken))
            .WithMessage("Customer not found.");

        }

        async Task<bool> ProductExists(Guid productId, CancellationToken cancellationToken)
        {
            if (productId == Guid.Empty)
                return false;

            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            return product != null;
        }

        async Task<bool> CustomerExists(Guid customerId, CancellationToken cancellationToken)
        {
            if (customerId == Guid.Empty)
                return false;

            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
            return customer != null;

        }
    }
}
