using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Products.Common;
using Application.Features.Products.GetList;
using AutoMapper;
using Domain.Entities;
using Moq;
using static Application.Features.Products.GetList.GetProductListQuery;

namespace UnitTests.Application.Features.Products
{
    public class GetProductListQueryHandlerTests
    {
        readonly Mock<IProductRepository> _productRepositoryMock;
        readonly Mock<IMapper> _mapperMock;
        readonly GetProductListQueryHandler _handler;

        public GetProductListQueryHandlerTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetProductListQuery.GetProductListQueryHandler(
                _productRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsPaginatedList()
        {
            var query = new GetProductListQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            var products = new List<Product>{
                new Product { Id = Guid.NewGuid(), Name = "Sample Product1", Description = "Sample Description1" },
                new Product { Id = Guid.NewGuid(),  Name = "Sample Product2",Description = "Sample Description2" }
            };
            
            var expectedProductResponses = products.Select(product => new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description
            });

            _mapperMock.Setup(mapper => mapper.ConfigurationProvider)
                    .Returns(new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductResponse>()));

            _productRepositoryMock.Setup(repo => repo.GetQueryable())
                                  .Returns(products.AsQueryable());
          
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(products.Count, result.Data.Count); 
            Assert.Equal(query.PageNumber, result.PageNumber);
        }
    }

}
