using Application.Common.Models;
using Application.Features.Customers.Common;
using Application.Features.Customers.Create;
using Application.Features.Orders.Common;
using Application.Features.Orders.Create;
using Application.Features.Orders.Update;
using Application.Features.Products.Common;
using Application.Features.Products.Create;
using Domain.Enums;
using Microsoft.DotNet.MSIdentity.Shared;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using Theoremone.SmartAc.Test.Infrastructure;
using WebAPI.Controllers;

namespace IntegrationTests.Controllers
{
    public class OrdersControllerTests : IDisposable
    {
        const string _apiBaseUrl = "api/orders";
        readonly WebApiApplication<OrdersController> _application;
        readonly HttpClient _client;

        ProductResponse  _defaultProductResponse;
        CustomerResponse _defaultCustomerResponse;

        public OrdersControllerTests()
        {
            _application = new WebApiApplication<OrdersController>();
            _client = _application.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _application.Dispose();
        }

        [Fact]
        public async Task CreateOrder_ReturnsNewOrder()
        {
            // Act
            var createdOrder = await CreateOrderAsync();

            // Assert
            Assert.NotNull(createdOrder);
            Assert.NotEqual(Guid.Empty, createdOrder.Id);
            Assert.Equal(_defaultProductResponse.Id, createdOrder.ProductId);
            Assert.Equal(_defaultCustomerResponse.Id, createdOrder.CustomerId);
            Assert.Equal(OrderStatus.Pending, createdOrder.Status);
        }

        [Fact]
        public async Task UpdateOrder_UpdatesExistingOrder()
        {
            var createdOrder = await CreateOrderAsync();
            var url = $"{_apiBaseUrl}/{createdOrder.Id}";
            var updateOrderRequest = new UpdateOrderRequest { Status = OrderStatus.Delivered };

            // Act
            var updateResponse = await _client.PutAsJsonAsync(url, updateOrderRequest);
            updateResponse.EnsureSuccessStatusCode();

            var response = await _client.GetStringAsync(url);
            var updatedOrderResponse = JsonConvert.DeserializeObject<OrderResponse>(response);
            Assert.Equal(updateOrderRequest.Status, updatedOrderResponse.Status);
        }

        [Fact]
        public async Task DeleteOrder_DeletesExistingOrder()
        {
            var createdOrder = await CreateOrderAsync();
            var url = $"{_apiBaseUrl}/{createdOrder.Id}";

            // Act
            var deleteResponse = await _client.DeleteAsync(url);
            deleteResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task DeleteOrder_ReturnsNotFound_WhenOrderNotExist()
        {
            var randomId = Guid.NewGuid();
            var url = $"{_apiBaseUrl}/{randomId}";

            // Act
            var deleteResponse = await _client.DeleteAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task GetOrder_ReturnsOrder()
        {
            var createdOrder = await CreateOrderAsync();

            // Act
            var url = $"{_apiBaseUrl}/{createdOrder.Id}";
            var getResponse = await _client.GetStringAsync(url);

            // Assert
            var retrievedOrder = JsonConvert.DeserializeObject<OrderResponse>(getResponse);

            Assert.NotNull(retrievedOrder);
            Assert.Equal(createdOrder.Id, retrievedOrder.Id);
            Assert.Equal(createdOrder.ProductId, retrievedOrder.ProductId);
            Assert.Equal(createdOrder.CustomerId, retrievedOrder.CustomerId);
            Assert.Equal(createdOrder.Status, retrievedOrder.Status);
        }

        [Fact]
        public async Task GetOrder_ReturnsNotFound_WhenOrderNotExist()
        {
            var randomId = Guid.NewGuid();
            var url = $"{_apiBaseUrl}/{randomId}";

            // Act
            var getResponse = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task GetList_ReturnsPaginatedOrderList()
        {
            var page = 1;
            var pageSize = 2;

            await CreateOrderAsync();
            await CreateOrderAsync();

            // Act
            var url = $"{_apiBaseUrl}/?pageNumber={page}&pageSize={pageSize}";
            var response = await _client.GetStringAsync(url);
            var pagedResponse = JsonConvert.DeserializeObject<PaginatedList<OrderResponse>>(response);

            // Assert
            Assert.NotNull(pagedResponse);
            Assert.NotNull(pagedResponse.Data);
            Assert.NotEmpty(pagedResponse.Data);
            Assert.Equal(page, pagedResponse.PageNumber);
            Assert.Equal(pageSize, pagedResponse.PageSize);
        }

        private async Task<OrderResponse> CreateOrderAsync(CreateOrderCommand createOrderRequest = null)
        {
            _defaultCustomerResponse = _defaultCustomerResponse ?? await CreateCustomerAsync();
            _defaultProductResponse = _defaultProductResponse ?? await CreateProductAsync();


            var request = createOrderRequest ?? new CreateOrderCommand
            {
                ProductId = _defaultProductResponse.Id,
                CustomerId = _defaultCustomerResponse.Id
            };

            var response = await _client.PostAsJsonAsync(_apiBaseUrl, request);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<OrderResponse>(jsonResponse);// await response.Content.ReadFromJsonAsync<OrderResponse>();
        }

        private async Task<CustomerResponse> CreateCustomerAsync(CreateCustomerCommand request = null)
        {
            var createCustomerRequest = request ?? new CreateCustomerCommand
            {
                FirstName = $"James_{Guid.NewGuid()}",
                LastName = $"Bond_{Guid.NewGuid()}",
                Phone = 1234567890,
                Email = $"{Guid.NewGuid()}.Bond@example.com"
            };

            var response = await _client.PostAsJsonAsync("api/customers", createCustomerRequest);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CustomerResponse>();
        }

        private async Task<ProductResponse> CreateProductAsync(CreateProductCommand createProductRequest = null)
        {
            var request = createProductRequest ?? new CreateProductCommand
            {
                Name = $"ProductName_{Guid.NewGuid()}",
                Description = $"ProductDescription_{Guid.NewGuid()}",
                SKU = 998877
            };

            var response = await _client.PostAsJsonAsync("api/products", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProductResponse>();
        }
    }
}
