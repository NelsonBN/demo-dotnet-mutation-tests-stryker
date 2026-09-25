using Demo.Application.DTOs;
using Demo.Application.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Application.UseCases;

public sealed class GetLowStockProductsUseCase(IProductsRepository repository)
{
    private readonly IProductsRepository _repository = repository;

    public async Task<IEnumerable<ProductResponse>> ExecuteAsync(uint threshold, CancellationToken cancellationToken = default)
        => (await _repository.ListAsync(cancellationToken))
            .Where(product => product.IsLowStock(threshold))
            .Select(product => (ProductResponse)product);
}
