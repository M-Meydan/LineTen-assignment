using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Common.Mappings;
using Application.Features.Customers.Common;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<CustomerResponse>, IMapTo<Customer>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; }
        

        public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, CustomerResponse>
        {
            private readonly ICustomerRepository _customerRepository;
            private readonly IValidator<CreateCustomerCommand> _validator;
            private readonly IMapper _mapper;

            public CreateCustomerHandler(ICustomerRepository customerRepository,
                IValidator<CreateCustomerCommand> validator,
                IMapper mapper,
                IMediator mediator)
            {
                _customerRepository = customerRepository;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<CustomerResponse> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(command);

                if (!validationResult.IsValid)
                    throw new ValidationException(validationResult.Errors);

                var entity = _mapper.Map<Customer>(command);

                var customer = await _customerRepository.AddAsync(entity, cancellationToken);

                return _mapper.Map<CustomerResponse>(customer);
            }
        }
    }
}
