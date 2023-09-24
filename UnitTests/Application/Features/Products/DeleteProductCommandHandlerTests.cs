using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Products.Delete;
using Domain.Entities;
using Moq;
using static Application.Features.Products.Delete.DeleteProductCommand;

namespace UnitTests.Application.Features.Products
{
    public class DeleteProductCommandHandlerTests
    {
        readonly Mock<IProductRepository> _productRepositoryMock;
        readonly DeleteProductHandler _handler;

        public DeleteProductCommandHandlerTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();

            _handler = new DeleteProductHandler(_productRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsDeletedProduct_WhenProductExist()
        {
            var id = Guid.NewGuid();
            var deleteRequest = new DeleteProductCommand { Id = id };

            var existingProduct = new Product
            {
                Id = id,
                Name = "Sample Product",
                Description = "Sample Description",
                SKU = 12345
            };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(existingProduct);

            // Act
            var result = await _handler.Handle(deleteRequest, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingProduct, result);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenProductNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync((Product)null);

            // Act
            var result = await _handler.Handle(new DeleteProductCommand { Id = productId }, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

    }
}
