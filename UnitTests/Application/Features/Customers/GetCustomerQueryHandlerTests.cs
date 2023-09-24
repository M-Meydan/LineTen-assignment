using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Customers.Common;
using Application.Features.Customers.Create;
using Application.Features.Customers.Get;
using Application.Features.Customers.Update;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Moq;


namespace UnitTests.Application.Features.Customers
{
    public class GetCustomerQueryHandlerTests
    {
        readonly Mock<ICustomerRepository> _customerRepositoryMock;
        readonly Mock<IValidator<GetCustomerQuery>> _validatorMock;
        readonly IMapper _mapper;
        readonly GetCustomerQuery.GetCustomerHandler _handler;

        public GetCustomerQueryHandlerTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _validatorMock = new Mock<IValidator<GetCustomerQuery>>();
            _mapper = new MapperConfiguration(cfg => { cfg.CreateMap<Customer, CustomerResponse>(); }).CreateMapper();


            _handler = new GetCustomerQuery.GetCustomerHandler(
                _customerRepositoryMock.Object,
                _validatorMock.Object,
                _mapper);
        }

        [Fact]
        public async Task Handle_ReturnsCustomerResponse_WhenQueryIsValid()
        {
            var Id = Guid.NewGuid();
            var query = new GetCustomerQuery();
            var expectedCustomerResponse = new CustomerResponse()
            {
                Id = Id,
                FirstName = "James",
                LastName = "Bond",
                Phone = 1234567890,
                Email = "James@Bond.com"
            };

            _validatorMock.Setup(validator => validator.Validate(It.IsAny<GetCustomerQuery>()))
                         .Returns(new ValidationResult());

            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(new Customer
                                 {
                                     Id = Id,
                                     FirstName = "James",
                                     LastName = "Bond",
                                     Phone = 1234567890,
                                     Email = "James@Bond.com"
                                 });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCustomerResponse.Id, result.Id);
            Assert.Equal(expectedCustomerResponse.FirstName, result.FirstName);
            Assert.Equal(expectedCustomerResponse.LastName, result.LastName);
            Assert.Equal(expectedCustomerResponse.Phone, result.Phone);
            Assert.Equal(expectedCustomerResponse.Email, result.Email);
        }

        [Fact]
        public async Task Handle_ThrowsValidationException_WhenQueryIsInvalid()
        {
            var invalidQuery = new GetCustomerQuery(Guid.Empty);

            _validatorMock.Setup(validator => validator.Validate(It.IsAny<GetCustomerQuery>()))
                         .Returns(new ValidationResult(new[] { new ValidationFailure("Id", "Id is required.") }));

            // Act and Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(invalidQuery, CancellationToken.None));
        }
    }

}
