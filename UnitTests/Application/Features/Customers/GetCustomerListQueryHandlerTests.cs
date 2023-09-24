using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Application.Features.Customers.Common;
using Application.Features.Customers.GetList;
using AutoMapper;
using Domain.Entities;
using Moq;
using static Application.Features.Customers.GetList.GetCustomerListQuery;

namespace UnitTests.Application.Features.Customers
{
    public class GetCustomerListQueryHandlerTests
    {
        readonly Mock<ICustomerRepository> _customerRepositoryMock;
        readonly Mock<IMapper> _mapperMock;
        readonly GetCustomerListQueryHandler _handler;

        public GetCustomerListQueryHandlerTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetCustomerListQuery.GetCustomerListQueryHandler(
                _customerRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsPaginatedList()
        {
            var query = new GetCustomerListQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            var customers = new List<Customer>{
                new Customer { Id = Guid.NewGuid(), FirstName = "James", LastName = "Bond" },
                new Customer { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" }
            };
            
            var expectedCustomerResponses = customers.Select(customer => new CustomerResponse
                                                    {
                                                        Id = customer.Id,
                                                        FirstName = customer.FirstName,
                                                        LastName = customer.LastName
                                                    });

            _mapperMock.Setup(mapper => mapper.ConfigurationProvider)
                    .Returns(new MapperConfiguration(cfg => cfg.CreateMap<Customer, CustomerResponse>()));

            _customerRepositoryMock.Setup(repo => repo.GetQueryable())
                                  .Returns(customers.AsQueryable());
          
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(customers.Count, result.Data.Count); // Ensure the expected number of items
            Assert.Equal(query.PageNumber, result.PageNumber);
        }
    }

}
