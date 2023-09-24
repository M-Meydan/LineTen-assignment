using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Orders.Update;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace UnitTests.Application.Features.Orders
{
    public class UpdateOrderCommandHandlerTests
    {
        readonly Mock<IOrderRepository> _orderRepositoryMock;
        readonly IMapper _mapper;
        readonly UpdateOrderCommand.UpdateOrderHandler _handler;

        public UpdateOrderCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _mapper = new MapperConfiguration(cfg => { cfg.CreateMap<UpdateOrderCommand, Order>(); }).CreateMapper();

            _handler = new UpdateOrderCommand.UpdateOrderHandler(
                _orderRepositoryMock.Object,
                _mapper);
        }

        [Fact]
        public async Task Handle_ReturnsUpdatedOrder()
        {
            var id = Guid.NewGuid();
            var updateCommand = new UpdateOrderCommand
            {   Id = id,
                Status = OrderStatus.Shipped
            };

            var existingOrder = new Order
            {
                Id = id,
                Status = OrderStatus.Pending
            };

            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(updateCommand.Id, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(existingOrder);

            // Act
            var result = await _handler.Handle(updateCommand, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateCommand.Id, result.Id);
            Assert.Equal(updateCommand.Status, result.Status);
            Assert.Equal(updateCommand.UpdatedDate, result.UpdatedDate);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenOrderNotExist()
        {
            var id = Guid.NewGuid();
            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync((Order)null);

            // Act
            var result = await _handler.Handle(new UpdateOrderCommand { Id = id }, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
