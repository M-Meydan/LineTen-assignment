using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Orders.Delete;
using Domain.Entities;
using Moq;

namespace UnitTests.Application.Features.Orders
{
    public class DeleteOrderCommandHandlerTests
    {
        readonly Mock<IOrderRepository> _orderRepositoryMock;
        readonly DeleteOrderCommand.DeleteOrderHandler _handler;

        public DeleteOrderCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();

            _handler = new DeleteOrderCommand.DeleteOrderHandler(_orderRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsDeletedOrder_WhenOrderExist()
        {
            var id = Guid.NewGuid();
            var deleteRequest = new DeleteOrderCommand { Id = id };

            var existingOrder = new Order
            {
                Id = id,
                ProductId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid()
            };

            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(existingOrder);

            // Act
            var result = await _handler.Handle(deleteRequest, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingOrder, result);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenOrderNotExist()
        {
            var id = Guid.NewGuid();
            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync((Order)null);

            // Act
            var result = await _handler.Handle(new DeleteOrderCommand { Id = id }, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

    }
}
