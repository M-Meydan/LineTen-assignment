using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Customers.Common;
using Application.Features.Customers.Create;
using Application.Features.Customers.Delete;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace UnitTests.Application.Features.Customers
{
    public class DeleteCustomerCommandHandlerTests
    {
        readonly Mock<ICustomerRepository> _customerRepositoryMock;
        readonly DeleteCustomerCommand.DeleteCustomerHandler _handler;

        public DeleteCustomerCommandHandlerTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();

            _handler = new DeleteCustomerCommand.DeleteCustomerHandler(_customerRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsDeletedCustomer_WhenCustomerExist()
        {
            var customerId = Guid.NewGuid();
            var deleteRequest = new DeleteCustomerCommand { Id = customerId };

            var existingCustomer = new Customer
            {
                Id = customerId,
                FirstName = "James",
                LastName = "Bond",
                Phone = 1234567890,
                Email = "James@example.com"
            };

            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(existingCustomer);

            // Act
            var result = await _handler.Handle(deleteRequest, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingCustomer, result);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenCustomerNotExist()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync((Customer)null);

            // Act
            var result = await _handler.Handle(new DeleteCustomerCommand { Id = customerId }, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

    }
}
