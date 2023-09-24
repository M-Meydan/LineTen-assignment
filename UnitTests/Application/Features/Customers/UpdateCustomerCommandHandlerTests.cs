using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Customers.Update;
using AutoMapper;
using Domain.Entities;
using Moq;

namespace UnitTests.Application.Features.Customers
{
    public class UpdateCustomerCommandHandlerTests
    {
        readonly Mock<ICustomerRepository> _customerRepositoryMock;
        readonly IMapper _mapper;
        readonly UpdateCustomerCommand.UpdateCustomerHandler _handler;

        public UpdateCustomerCommandHandlerTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _mapper = new MapperConfiguration(cfg => { cfg.CreateMap<UpdateCustomerCommand, Customer>(); }).CreateMapper();

            _handler = new UpdateCustomerCommand.UpdateCustomerHandler(
                _customerRepositoryMock.Object,
                _mapper);
        }

        [Fact]
        public async Task Handle_ReturnsUpdatedCustomer()
        {
            var updateCommand = new UpdateCustomerCommand
            {   Id = Guid.NewGuid(),
                FirstName = "Updated",
                LastName = "Customer",
                Phone = 1234567890,
                Email = "updated@example.com"
            };

            var existingCustomer = new Customer
            {
                Id = updateCommand.Id,
                FirstName = "Original",
                LastName = "Customer",
                Phone = 987654321,
                Email = "original@example.com"
            };

            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(updateCommand.Id, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(existingCustomer);

            // Act
            var result = await _handler.Handle(updateCommand, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateCommand.Id, result.Id);
            Assert.Equal(updateCommand.FirstName, result.FirstName);
            Assert.Equal(updateCommand.LastName, result.LastName);
            Assert.Equal(updateCommand.Email, result.Email);
            Assert.Equal(updateCommand.Phone, result.Phone);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenCustomerNotExist()
        {
            var customerId = Guid.NewGuid();
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync((Customer)null);

            // Act
            var result = await _handler.Handle(new UpdateCustomerCommand { Id = customerId }, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
