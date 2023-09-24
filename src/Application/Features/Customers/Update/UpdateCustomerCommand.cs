using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Customers.Update
{
    public class UpdateCustomerCommand : IRequest<Customer>, IMapTo<Customer>
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; }


        public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, Customer>
        {
            readonly ICustomerRepository _customerRepository;
            readonly IMapper _mapper;
            public UpdateCustomerHandler(ICustomerRepository customerRepository, IMapper mapper)
            {
                _customerRepository = customerRepository;
                _mapper = mapper;
            }

            public async Task<Customer> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
            {
                var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);

                if (customer != null)
                {
                    var updatedcustomer = _mapper.Map(request, customer);
                    await _customerRepository.UpdateAsync(updatedcustomer,cancellationToken);
                }

                return customer;
            }
        }
    }
}
