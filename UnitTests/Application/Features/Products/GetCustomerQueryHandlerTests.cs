using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Products.Common;
using Application.Features.Products.Get;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Moq;


namespace UnitTests.Application.Features.Products
{
    public class GetProductQueryHandlerTests
    {
        readonly Mock<IProductRepository> _productRepositoryMock;
        readonly Mock<IValidator<GetProductQuery>> _validatorMock;
        readonly IMapper _mapper;
        readonly GetProductQuery.GetProductHandler _handler;

        public GetProductQueryHandlerTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _validatorMock = new Mock<IValidator<GetProductQuery>>();
            _mapper = new MapperConfiguration(cfg => { cfg.CreateMap<Product, ProductResponse>(); }).CreateMapper();


            _handler = new GetProductQuery.GetProductHandler(
                _productRepositoryMock.Object,
                _validatorMock.Object,
                _mapper);
        }

        [Fact]
        public async Task Handle_ReturnsProductResponse_WhenQueryIsValid()
        {
            var Id = Guid.NewGuid();
            var query = new GetProductQuery();
            var expectedProductResponse = new ProductResponse()
            {
                Id = Id,
                Name = "Sample Product",
                Description = "Sample Description",
                SKU = 12345
            };

            _validatorMock.Setup(validator => validator.Validate(It.IsAny<GetProductQuery>()))
                         .Returns(new ValidationResult());

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(new Product
                                 {
                                     Id = Id,
                                     Name = "Sample Product",
                                     Description = "Sample Description",
                                     SKU = 12345
                                 });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProductResponse.Id, result.Id);
            Assert.Equal(expectedProductResponse.Name, result.Name);
            Assert.Equal(expectedProductResponse.Description, result.Description);
            Assert.Equal(expectedProductResponse.SKU, result.SKU);
        }

        [Fact]
        public async Task Handle_ThrowsValidationException_WhenQueryIsInvalid()
        {
            var invalidQuery = new GetProductQuery(Guid.Empty);

            _validatorMock.Setup(validator => validator.Validate(It.IsAny<GetProductQuery>()))
                         .Returns(new ValidationResult(new[] { new ValidationFailure("Id", "Id is required.") }));

            // Act and Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(invalidQuery, CancellationToken.None));
        }
    }

}
