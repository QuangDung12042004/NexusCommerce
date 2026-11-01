using Microsoft.EntityFrameworkCore;
using NexusCommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NexusCommerce.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Product> Products { get; }
        DbSet<Inventory> Inventories { get; }
        DbSet<Order> Orders { get; }
        DbSet<OrderItems> OrderItems { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}