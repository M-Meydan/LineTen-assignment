using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Customers.Common;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Application.Features.Customers.Queries.GetCustomer
{
    public class GetCustomerQuery : IRequest<CustomerResponse>
    {
        /// <summary>
        /// Id
        /// </summary>
        /// <example>GUID</example>
        public Guid Id { get; set; }

        public GetCustomerQuery() { }
        public GetCustomerQuery(Guid id)
        {
            Id = id;
        }

        public class GetCustomerHandler : IRequestHandler<GetCustomerQuery, CustomerResponse>
        {
            private readonly ICustomerRepository _customerRepository;
            private readonly IValidator<GetCustomerQuery> _validator;
            private readonly IMapper _mapper;

            public GetCustomerHandler(ICustomerRepository customerRepository,
                IValidator<GetCustomerQuery> validator,
                IMapper mapper)
            {
                _customerRepository = customerRepository;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<CustomerResponse> Handle(GetCustomerQuery query, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(query);

                if (!validationResult.IsValid)
                    throw new ValidationException(validationResult.Errors);

                var customer = await _customerRepository.GetByIdAsync(query.Id);
                return _mapper.Map<CustomerResponse>(customer);
            }
        }
    }
}
