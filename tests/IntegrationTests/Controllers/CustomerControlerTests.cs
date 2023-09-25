using Application.Common.Models;
using Application.Features.Customers.Common;
using Application.Features.Customers.Create;
using Application.Features.Customers.Update;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Configuration;
using System.Net;
using System.Net.Http.Json;
using Theoremone.SmartAc.Test.Infrastructure;
using WebAPI.Controllers;

namespace IntegrationTests.Controllers
{
    public class CustomerControlerTests : IDisposable
    {
        const string _apiBaseUrl = "api/customers";
        readonly WebApiApplication<CustomersController> _application;
        readonly HttpClient _client;

        public CustomerControlerTests()
        {
            _application = new WebApiApplication<CustomersController>();
            _client = _application.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _application.Dispose();
        }

        [Fact]
        public async Task CreateCustomer_ReturnsNewCustomer()
        {
            var createCustomerRequest = new CreateCustomerCommand
            {
                FirstName = "James",
                LastName = "Bond",
                Phone = 1234567890,
                Email = "James.Bond@example.com"
            };

            // Act
            var createdCustomer = await CreateCustomerAsync(createCustomerRequest);

            // Assert

            Assert.NotNull(createdCustomer);
            Assert.NotEqual(Guid.Empty, createdCustomer.Id);
            Assert.Equal(createCustomerRequest.FirstName, createdCustomer.FirstName);
            Assert.Equal(createCustomerRequest.LastName, createdCustomer.LastName);
            Assert.Equal(createCustomerRequest.Phone, createdCustomer.Phone);
            Assert.Equal(createCustomerRequest.Email, createdCustomer.Email);
        }

        [Fact]
        public async Task UpdateCustomer_UpdatesExistingCustomer()
        {
            var createdCustomer = await CreateCustomerAsync();

            // Act
            var url = $"{_apiBaseUrl}/{createdCustomer.Id}";
            var updateCustomerRequest = new UpdateCustomerRequest
            {
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                Phone = 1876543210,
                Email = "updated.email@example.com"
            };

            var updateResponse = await _client.PutAsJsonAsync(url, updateCustomerRequest);
            updateResponse.EnsureSuccessStatusCode();

            var updatedCustomerResponse = await _client.GetFromJsonAsync<CustomerResponse>(url);

            Assert.Equal(updateCustomerRequest.FirstName, updatedCustomerResponse.FirstName);
            Assert.Equal(updateCustomerRequest.LastName, updatedCustomerResponse.LastName);
            Assert.Equal(updateCustomerRequest.Phone, updatedCustomerResponse.Phone);
            Assert.Equal(updateCustomerRequest.Email, updatedCustomerResponse.Email);
        }


        [Fact]
        public async Task DeleteCustomer_DeletesExistingCustomer()
        {
            var createdCustomer = await CreateCustomerAsync();

            // Act
            var url = $"{_apiBaseUrl}/{createdCustomer.Id}";
            var deleteResponse = await _client.DeleteAsync(url);
            deleteResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsNotFound_WhenCustomerNotExist()
        {
            var randomId = Guid.NewGuid();
            var url = $"{_apiBaseUrl}/{randomId}";

            // Act
            var deleteResponse = await _client.DeleteAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        }


        [Fact]
        public async Task GetCustomer_ReturnsCustomer()
        {
            var createdCustomer = await CreateCustomerAsync();

            // Act
            var url = $"{_apiBaseUrl}/{createdCustomer.Id}";
            var getResponse = await _client.GetAsync(url);

            // Assert
            getResponse.EnsureSuccessStatusCode();
            var retrievedCustomer = await getResponse.Content.ReadFromJsonAsync<CustomerResponse>();

            Assert.NotNull(retrievedCustomer);
            Assert.Equal(createdCustomer.Id, retrievedCustomer.Id);
            Assert.Equal(createdCustomer.FirstName, retrievedCustomer.FirstName);
            Assert.Equal(createdCustomer.LastName, retrievedCustomer.LastName);
            Assert.Equal(createdCustomer.Phone, retrievedCustomer.Phone);
            Assert.Equal(createdCustomer.Email, retrievedCustomer.Email);
        }

        [Fact]
        public async Task GetCustomer_ReturnsNotFound_WhenCustomerNotExist()
        {
            var randomId = Guid.NewGuid();
            var url = $"{_apiBaseUrl}/{randomId}";

            // Act
            var getResponse = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }


        [Fact]
        public async Task GetList_ReturnsPaginatedCustomerList()
        {
            var page = 1;
            var pageSize = 2;

            await CreateCustomerAsync();
            await CreateCustomerAsync();

            // Act
            var url = $"{_apiBaseUrl}/?pageNumber={page}&pageSize={pageSize}";
            var pagedResponse = await _client.GetFromJsonAsync<PaginatedList<CustomerResponse>>(url);

            // Assert
            Assert.NotNull(pagedResponse);
            Assert.NotNull(pagedResponse.Data);
            Assert.NotEmpty(pagedResponse.Data);
            Assert.Equal(page, pagedResponse.PageNumber); // Replace with your expected page number
            Assert.Equal(pageSize, pagedResponse.PageSize); // Replace with your expected page size
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

            var response = await _client.PostAsJsonAsync(_apiBaseUrl, createCustomerRequest);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CustomerResponse>();
        }

    }
}
