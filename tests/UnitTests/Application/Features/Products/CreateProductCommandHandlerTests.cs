using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Products.Common;
using Application.Features.Products.Create;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;

namespace UnitTests.Application.Features.Products
{
    public class CreateProductCommandHandlerTests
    {
        readonly Mock<IProductRepository> _customerRepositoryMock;
        readonly Mock<IValidator<CreateProductCommand>> _validatorMock;
        readonly Mock<IMapper> _mapperMock;
        readonly CreateProductCommand.CreateProductHandler _handler;

        public CreateProductCommandHandlerTests()
        {
            _customerRepositoryMock = new Mock<IProductRepository>();
            _validatorMock = new Mock<IValidator<CreateProductCommand>>();
            _mapperMock = new Mock<IMapper>();

            _handler = new CreateProductCommand.CreateProductHandler(
                _customerRepositoryMock.Object,
                _validatorMock.Object,
                _mapperMock.Object,
                Mock.Of<IMediator>());
        }

        [Fact]
        public async Task Handle_ReturnsProductResponse_WhenCommandIsValid()
        {
            var command = new CreateProductCommand
            {
                Name = "Sample Product",
                Description = "Sample Description",
                SKU = 12345
            };

            var expectedProductResponse = new ProductResponse()
            {
                Id = Guid.NewGuid(),
                Name=command.Name,
                Description=command.Description,
                SKU=command.SKU
            };

            _validatorMock.Setup(validator => validator.Validate(It.IsAny<CreateProductCommand>()))
                         .Returns(new ValidationResult());

            _mapperMock.Setup(mapper => mapper.Map<Product>(It.IsAny<CreateProductCommand>()))
                      .Returns(new Product());

            _mapperMock.Setup(mapper => mapper.Map<ProductResponse>(It.IsAny<Product>()))
                      .Returns(expectedProductResponse);

            _customerRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(new Product());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.Name, result.Name);
            Assert.Equal(command.Description, result.Description);
            Assert.Equal(command.SKU, result.SKU);
            

            _customerRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
            _validatorMock.Verify(validator => validator.Validate(It.IsAny<CreateProductCommand>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<Product>(It.IsAny<CreateProductCommand>()), Times.Once);

        }

        [Theory]
        [InlineData(null, "Description", 12345)]
        [InlineData("Sample Product", null, 12345)]
        [InlineData("Sample Product", "Description", 0)] 
        public async Task Handle_ThrowsValidationException_WhenCommandIsInvalid(
            string name, string description, int sku)
        {
            var invalidCommand = new CreateProductCommand
            {
                Name = name,
                Description = description,
                SKU = sku
            };

            _validatorMock.Setup(validator => validator.Validate(It.IsAny<CreateProductCommand>()))
                         .Returns(new ValidationResult(new[] { new ValidationFailure("field", "Field is required.") }));

            // Act and Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(invalidCommand, CancellationToken.None));
        }

    }
}
