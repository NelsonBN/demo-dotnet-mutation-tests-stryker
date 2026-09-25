using Demo.Application.DTOs;
using Demo.Application.Services;
using Demo.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Application.UseCases;

public sealed record GetProductUseCase(IProductsRepository repository)
{
    private readonly IProductsRepository _repository = repository;

    public async Task<ProductResponse> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetAsync(id, cancellationToken);
        if (product is null)
        {
            throw new NotFoundException("Product not found.");
        }

        return product;
    }
}
