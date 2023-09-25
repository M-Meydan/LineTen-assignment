using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Orders.Common;
using Application.Features.Orders.Create;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;

namespace UnitTests.Application.Features.Orders
{
    public class CreateOrderCommandHandlerTests
    {
        readonly Mock<IOrderRepository> _orderRepositoryMock;
        readonly Mock<IProductRepository> _productRepositoryMock;
        readonly Mock<ICustomerRepository> _customerRepositoryMock;
        readonly Mock<IValidator<CreateOrderCommand>> _validatorMock;
        readonly Mock<IMapper> _mapperMock;
        readonly CreateOrderCommand.CreateOrderHandler _handler;

        public CreateOrderCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();

            _validatorMock = new Mock<IValidator<CreateOrderCommand>>();
            _mapperMock = new Mock<IMapper>();

            _handler = new CreateOrderCommand.CreateOrderHandler(
                _orderRepositoryMock.Object,
                _validatorMock.Object,
                _mapperMock.Object,
                Mock.Of<IMediator>());
        }

        [Fact]
        public async Task Handle_ReturnsOrderResponse_WhenCommandIsValid()
        {
            var command = new CreateOrderCommand
            {
                ProductId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid()
            };

            var expectedOrder = new Order
            {
                Id = Guid.NewGuid(),
                ProductId = command.ProductId,
                CustomerId = command.CustomerId
            };

            var expectedOrderResponse = new OrderResponse
            {
                Id = expectedOrder.Id,
                ProductId = expectedOrder.ProductId,
                CustomerId = expectedOrder.CustomerId,
                Status = expectedOrder.Status,
                CreatedDate = expectedOrder.CreatedDate,
                UpdatedDate = expectedOrder.UpdatedDate
            };

            _validatorMock.Setup(validator => validator.ValidateAsync(It.IsAny<CreateOrderCommand>(), CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

            _orderRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync(expectedOrder);

            _mapperMock.Setup(mapper => mapper.Map<OrderResponse>(It.IsAny<Order>()))
                      .Returns(expectedOrderResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedOrderResponse.Id, result.Id);
            Assert.Equal(expectedOrderResponse.ProductId, result.ProductId);
            Assert.Equal(expectedOrderResponse.CustomerId, result.CustomerId);
            Assert.Equal(expectedOrderResponse.Status, result.Status);
            Assert.Equal(expectedOrderResponse.CreatedDate, result.CreatedDate);
            Assert.Equal(expectedOrderResponse.UpdatedDate, result.UpdatedDate);

            _orderRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
            _validatorMock.Verify(validator => validator.ValidateAsync(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<OrderResponse>(It.IsAny<Order>()), Times.Once);
        }

        [Theory]
        [InlineData(null, "CustomerId123")]
        [InlineData("ProductId123", null)]
        [InlineData("ProductId123", "CustomerId123")]
        public async Task Handle_ThrowsValidationException_WhenCommandIsInvalid(string productId, string customerId)
        {
            var invalidCommand = new CreateOrderCommand
            {
                ProductId = string.IsNullOrEmpty(productId) ? Guid.Empty : Guid.NewGuid(),
                CustomerId = string.IsNullOrEmpty(customerId) ? Guid.Empty : Guid.NewGuid(),
            };

            _validatorMock.Setup(validator => validator.ValidateAsync(It.IsAny<CreateOrderCommand>(), CancellationToken.None))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("field", "Field is required.") }));


            // Act and Assert: Ensure that a ValidationException is thrown
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(invalidCommand, CancellationToken.None));
        }

    }
}
