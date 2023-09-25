using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Products.Update;
using AutoMapper;
using Domain.Entities;
using Moq;

namespace UnitTests.Application.Features.Products
{
    public class UpdateProductCommandHandlerTests
    {
        readonly Mock<IProductRepository> _productRepositoryMock;
        readonly IMapper _mapper;
        readonly UpdateProductCommand.UpdateProductHandler _handler;

        public UpdateProductCommandHandlerTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _mapper = new MapperConfiguration(cfg => { cfg.CreateMap<UpdateProductCommand, Product>(); }).CreateMapper();

            _handler = new UpdateProductCommand.UpdateProductHandler(
                _productRepositoryMock.Object,
                _mapper);
        }

        [Fact]
        public async Task Handle_ReturnsUpdatedProduct()
        {
            var updateCommand = new UpdateProductCommand
            {   Id = Guid.NewGuid(),
                Name = "Sample Product",
                Description = "Sample Description"
            };

            var existingProduct = new Product
            {
                Id = updateCommand.Id,
                Name = "Original Product",
                Description = "Original Description",
                SKU = 12345
            };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(updateCommand.Id, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(existingProduct);

            // Act
            var result = await _handler.Handle(updateCommand, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateCommand.Id, result.Id);
            Assert.Equal(updateCommand.Name, result.Name);
            Assert.Equal(updateCommand.Description, result.Description);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenProductNotExist()
        {
            var customerId = Guid.NewGuid();
            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync((Product)null);

            // Act
            var result = await _handler.Handle(new UpdateProductCommand { Id = customerId }, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
