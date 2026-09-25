using Demo.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Application.Services;

public interface IProductsRepository
{
    Task<IEnumerable<Product>> ListAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string? name, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, string? name, CancellationToken cancellationToken = default);
}