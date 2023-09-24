using Application.Common.Interfaces.Infrastructure.DI;
using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Domain.Entities;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository, IScopedDependency
    {
        readonly DbSet<Customer> _customers;

        public CustomerRepository(AppDbContext dbContext) : base(dbContext)
        {
            _customers = dbContext.Set<Customer>();
        }
    }
}
