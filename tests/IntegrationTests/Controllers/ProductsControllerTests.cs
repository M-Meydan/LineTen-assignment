using Application.Common.Models;
using Application.Features.Products.Common;
using Application.Features.Products.Create;
using Application.Features.Products.Update;
using System.Net;
using System.Net.Http.Json;
using Theoremone.SmartAc.Test.Infrastructure;
using WebAPI.Controllers;

namespace IntegrationTests.Controllers
{
    public class ProductsControllerTests : IDisposable
    {
        const string _apiBaseUrl = "api/products";
        readonly WebApiApplication<ProductsController> _application;
        readonly HttpClient _client;

        public ProductsControllerTests()
        {
            _application = new WebApiApplication<ProductsController>();
            _client = _application.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _application.Dispose();
        }

        [Fact]
        public async Task CreateProduct_ReturnsNewProduct()
        {
            var createProductRequest = new CreateProductCommand
            {
                Name = "Product Name",
                Description = "Product Description",
                SKU = 12345
            };

            // Act
            var createdProduct = await CreateProductAsync(createProductRequest);

            // Assert
            Assert.NotNull(createdProduct);
            Assert.NotEqual(Guid.Empty, createdProduct.Id);
            Assert.Equal(createProductRequest.Name, createdProduct.Name);
            Assert.Equal(createProductRequest.Description, createdProduct.Description);
            Assert.Equal(createProductRequest.SKU, createdProduct.SKU);
        }

        [Fact]
        public async Task UpdateProduct_UpdatesExistingProduct()
        {
            var createdProduct = await CreateProductAsync();

            // Act
            var url = $"{_apiBaseUrl}/{createdProduct.Id}";
            var updateProductRequest = new UpdateProductRequest
            {
                Name = "Updated Product Name",
                Description = "Updated Product Description"
            };

            var updateResponse = await _client.PutAsJsonAsync(url, updateProductRequest);
            updateResponse.EnsureSuccessStatusCode();

            var updatedProductResponse = await _client.GetFromJsonAsync<ProductResponse>(url);

            Assert.Equal(updateProductRequest.Name, updatedProductResponse.Name);
            Assert.Equal(updateProductRequest.Description, updatedProductResponse.Description);
        }

        [Fact]
        public async Task DeleteProduct_DeletesExistingProduct()
        {
            var createdProduct = await CreateProductAsync();
            var url = $"{_apiBaseUrl}/{createdProduct.Id}";

            // Act
            var deleteResponse = await _client.DeleteAsync(url);
            deleteResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenProductNotExist()
        {
            var randomId = Guid.NewGuid();
            var url = $"{_apiBaseUrl}/{randomId}";

            // Act
            var deleteResponse = await _client.DeleteAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task GetProduct_ReturnsProduct()
        {
            var createdProduct = await CreateProductAsync();
            var url = $"{_apiBaseUrl}/{createdProduct.Id}";

            // Act
            var getResponse = await _client.GetAsync(url);

            // Assert
            getResponse.EnsureSuccessStatusCode();
            var retrievedProduct = await getResponse.Content.ReadFromJsonAsync<ProductResponse>();

            Assert.NotNull(retrievedProduct);
            Assert.Equal(createdProduct.Id, retrievedProduct.Id);
            Assert.Equal(createdProduct.Name, retrievedProduct.Name);
            Assert.Equal(createdProduct.Description, retrievedProduct.Description);
            Assert.Equal(createdProduct.SKU, retrievedProduct.SKU);
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenProductNotExist()
        {
            var randomId = Guid.NewGuid();
            var url = $"{_apiBaseUrl}/{randomId}";

            // Act
            var getResponse = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task GetList_ReturnsPaginatedProductList()
        {
            var page = 1;
            var pageSize = 2;

            await CreateProductAsync();
            await CreateProductAsync();

            // Act
            var url = $"{_apiBaseUrl}/?pageNumber={page}&pageSize={pageSize}";
            var pagedResponse = await _client.GetFromJsonAsync<PaginatedList<ProductResponse>>(url);

            // Assert
            Assert.NotNull(pagedResponse);
            Assert.NotNull(pagedResponse.Data);
            Assert.NotEmpty(pagedResponse.Data);
            Assert.Equal(page, pagedResponse.PageNumber);
            Assert.Equal(pageSize, pagedResponse.PageSize);
        }

        private async Task<ProductResponse> CreateProductAsync(CreateProductCommand createProductRequest = null)
        {
            var request = createProductRequest ?? new CreateProductCommand
            {
                Name = $"ProductName_{Guid.NewGuid()}",
                Description = $"ProductDescription_{Guid.NewGuid()}",
                SKU = 998877
            };

            var response = await _client.PostAsJsonAsync(_apiBaseUrl, request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProductResponse>();
        }
    }

}
