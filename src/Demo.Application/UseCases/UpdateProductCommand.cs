using Demo.Application.DTOs;
using Demo.Application.Services;
using Demo.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Application.UseCases;

public sealed record UpdateProductUseCase(IProductsRepository repository)
{
    private readonly IProductsRepository _repository = repository;

    public async Task ExecuteAsync(Guid id, ProductRequest request, CancellationToken cancellationToken = default)
    {
        if (await _repository.ExistsAsync(id, request.Name, cancellationToken))
        {
            throw new DuplicateException("Product already exists.");
        }

        var product = await _repository.GetAsync(id, cancellationToken);
        if (product is null)
        {
            throw new NotFoundException("Product not found.");
        }

        product.Update(
            request.Name,
            request.Quantity);

        await _repository.UpdateAsync(product, cancellationToken);
    }
}
