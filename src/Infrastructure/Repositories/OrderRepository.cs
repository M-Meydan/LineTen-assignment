using Application.Common.Interfaces.Infrastructure.DI;
using Application.Common.Interfaces.Infrastructure.Persistence.Repositories;
using Domain.Entities;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository, IScopedDependency
    {
        readonly DbSet<Order> _orders;

        public OrderRepository(AppDbContext dbContext) : base(dbContext)
        {
            _orders = dbContext.Set<Order>();
        }
    }
}
