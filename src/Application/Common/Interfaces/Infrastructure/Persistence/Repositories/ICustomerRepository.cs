using Application.Common.Interfaces.Infrastructure.DI;
using Domain.Entities;

namespace Application.Common.Interfaces.Infrastructure.Persistence.Repositories
{
    public interface ICustomerRepository : IGenericRepository<Customer> { }
}
