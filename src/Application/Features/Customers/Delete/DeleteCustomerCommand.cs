using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.Customers.Delete
{
    public class DeleteCustomerCommand : IRequest<Customer>
    {
        public Guid Id { get; set; }

        public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand, Customer>
        {
            private readonly ICustomerRepository _customerRepository;

            public DeleteCustomerHandler(ICustomerRepository customerRepository)
            {
                _customerRepository = customerRepository;
            }

            public async Task<Customer> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
            {
                var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);

                if (customer != null)
                    await _customerRepository.DeleteAsync(customer);

                return customer;
            }
        }
    }
}
