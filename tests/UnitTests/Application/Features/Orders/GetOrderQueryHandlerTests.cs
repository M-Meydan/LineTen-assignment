using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Orders.Common;
using Application.Features.Orders.Get;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using Moq;


namespace UnitTests.Application.Features.Orders
{
    public class GetOrderQueryHandlerTests
    {
        readonly Mock<IOrderRepository> _orderRepositoryMock;
        readonly Mock<IValidator<GetOrderQuery>> _validatorMock;
        readonly Mock<IMapper> _mapperMock;
        readonly IMapper _mapper;
        readonly GetOrderQuery.GetOrderHandler _handler;

        public GetOrderQueryHandlerTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _validatorMock = new Mock<IValidator<GetOrderQuery>>();
            _mapper = new MapperConfiguration(cfg => { cfg.CreateMap<Order, OrderResponse>(); }).CreateMapper();
           
            _handler = new GetOrderQuery.GetOrderHandler(
                _orderRepositoryMock.Object,
                _validatorMock.Object,
                _mapper);
        }

        [Fact]
        public async Task Handle_ReturnsOrderResponse_WhenQueryIsValid()
        {
            var Id = Guid.NewGuid();
            var expectedOrder = new Order
            {
                Id = Id,
                ProductId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Status = OrderStatus.Pending,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            _validatorMock.Setup(validator => validator.Validate(It.IsAny<GetOrderQuery>()))
                         .Returns(new ValidationResult());

            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync(expectedOrder);

            // Act
            var result = await _handler.Handle(new GetOrderQuery(Id), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedOrder.Id, result.Id);
            Assert.Equal(expectedOrder.ProductId, result.ProductId);
            Assert.Equal(expectedOrder.CustomerId, result.CustomerId);
            Assert.Equal(expectedOrder.Status, result.Status);
            Assert.Equal(expectedOrder.CreatedDate, result.CreatedDate);
            Assert.Equal(expectedOrder.UpdatedDate, result.UpdatedDate);

        }

        [Fact]
        public async Task Handle_ThrowsValidationException_WhenQueryIsInvalid()
        {
            var invalidQuery = new GetOrderQuery(Guid.Empty);

            _validatorMock.Setup(validator => validator.Validate(It.IsAny<GetOrderQuery>()))
                         .Returns(new ValidationResult(new[] { new ValidationFailure("Id", "Id is required.") }));

            // Act and Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(invalidQuery, CancellationToken.None));
        }
    }

}
