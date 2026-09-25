using Demo.Application.Services;
using Demo.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Application.UseCases;

public sealed class DeleteProductUseCase(IProductsRepository repository)
{
    private readonly IProductsRepository _repository = repository;

    public async Task ExecuteAsync(Guid Id, CancellationToken cancellationToken = default)
    {
        if (!await _repository.ExistsAsync(Id))
        {
            throw new NotFoundException("Product not found.");
        }

        await _repository.DeleteAsync(Id, cancellationToken);
    }
}
