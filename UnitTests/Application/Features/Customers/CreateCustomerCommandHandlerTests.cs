using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Customers.Commands.CreateCustomer;
using Application.Features.Customers.Common;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Application.Features.Customers
{
    public class CreateCustomerCommandHandlerTests
    {
        readonly Mock<ICustomerRepository> _customerRepositoryMock;
        readonly Mock<IValidator<CreateCustomerCommand>> _validatorMock;
        readonly Mock<IMapper> _mapperMock;
        readonly CreateCustomerCommand.CreateCustomerHandler _handler;

        public CreateCustomerCommandHandlerTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _validatorMock = new Mock<IValidator<CreateCustomerCommand>>();
            _mapperMock = new Mock<IMapper>();

            _handler = new CreateCustomerCommand.CreateCustomerHandler(
                _customerRepositoryMock.Object,
                _validatorMock.Object,
                _mapperMock.Object,
                Mock.Of<IMediator>());
        }

        [Fact]
        public async Task Handle_ReturnsCustomerResponse_WhenCommandIsValid()
        {
            var command = new CreateCustomerCommand
            {
                FirstName = "James",
                LastName = "Bond",
                Phone = 1234567890, 
                Email = "James@Bond.com"
            };

            var expectedCustomerResponse = new CustomerResponse()
            {
                Id = Guid.NewGuid(),
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                Phone = command.Phone
            };
            
            _validatorMock.Setup(validator => validator.Validate(It.IsAny<CreateCustomerCommand>()))
                         .Returns(new ValidationResult());
            
            _mapperMock.Setup(mapper => mapper.Map<Customer>(It.IsAny<CreateCustomerCommand>()))
                      .Returns(new Customer());

            _mapperMock.Setup(mapper => mapper.Map<CustomerResponse>(It.IsAny<Customer>()))
                      .Returns(expectedCustomerResponse);

            _customerRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(new Customer());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.FirstName, result.FirstName); 
            Assert.Equal(command.LastName, result.LastName);   
            Assert.Equal(command.Phone, result.Phone);         
            Assert.Equal(command.Email, result.Email);         

            _customerRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once); 
            _validatorMock.Verify(validator => validator.Validate(It.IsAny<CreateCustomerCommand>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<Customer>(It.IsAny<CreateCustomerCommand>()), Times.Once); 

        }

        [Theory]
        [InlineData(null, "Bond", 1234567890, "James.Bond@example.com")]
        [InlineData("James", null, 1234567890, "James.Bond@example.com")]
        [InlineData("James", "Bond", 0, "James.Bond@example.com")] 
        [InlineData("James", "Bond", 1234567890, "invalid-email")] 
        public async Task Handle_ThrowsValidationException_WhenCommandIsInvalid(
        string firstName, string lastName, int phone, string email)
        {
            var invalidCommand = new CreateCustomerCommand
            {
                FirstName = firstName,
                LastName = lastName,
                Phone = phone,
                Email = email
            };

            _validatorMock.Setup(validator => validator.Validate(It.IsAny<CreateCustomerCommand>()))
                         .Returns(new ValidationResult(new[]
                         {
                         new ValidationFailure("FirstName", "FirstName is required."),
                             // Add more validation failures as needed for each field
                         }));

            // Act and Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(invalidCommand, CancellationToken.None));
        }

    }
}
