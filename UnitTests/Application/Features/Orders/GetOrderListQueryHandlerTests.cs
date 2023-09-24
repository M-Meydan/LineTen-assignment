using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Orders.Common;
using Application.Features.Orders.GetList;
using AutoMapper;
using Domain.Entities;
using Moq;
using static Application.Features.Orders.GetList.GetOrderListQuery;

namespace UnitTests.Application.Features.Orders
{
    public class GetOrderListQueryHandlerTests
    {
        readonly Mock<IOrderRepository> _orderRepositoryMock;
        readonly Mock<IMapper> _mapperMock;
        readonly GetOrderListQueryHandler _handler;

        public GetOrderListQueryHandlerTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetOrderListQuery.GetOrderListQueryHandler(
                _orderRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsPaginatedList()
        {
            var query = new GetOrderListQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            var orders = new List<Order>{
                new Order { Id = Guid.NewGuid(),  ProductId = Guid.NewGuid(), CustomerId = Guid.NewGuid(), },
                new Order { Id = Guid.NewGuid(),  ProductId = Guid.NewGuid(), CustomerId = Guid.NewGuid(), }
            };
            
            var expectedOrderResponses = orders.Select(customer => new OrderResponse
            {
                Id = customer.Id,
                ProductId = customer.ProductId,
                CustomerId = customer.CustomerId
            });

            _mapperMock.Setup(mapper => mapper.ConfigurationProvider)
                    .Returns(new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderResponse>()));

            _orderRepositoryMock.Setup(repo => repo.GetQueryable())
                                  .Returns(orders.AsQueryable());
          
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(orders.Count, result.Data.Count); 
            Assert.Equal(query.PageNumber, result.PageNumber);
        }
    }

}
