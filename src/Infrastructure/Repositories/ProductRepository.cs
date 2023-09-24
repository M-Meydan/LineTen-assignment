using Application.Common.Interfaces.Infrastructure.DI;
using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Domain.Entities;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository, IScopedDependency
    {
        readonly DbSet<Product> _customers;

        public ProductRepository(AppDbContext dbContext) : base(dbContext)
        {
            _customers = dbContext.Set<Product>();
        }
    }
}
