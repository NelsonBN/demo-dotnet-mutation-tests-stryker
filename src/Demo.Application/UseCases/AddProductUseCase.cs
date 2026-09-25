using Demo.Application.DTOs;
using Demo.Application.Services;
using Demo.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.UseCases;

public sealed class AddProductUseCase(IProductsRepository repository)
{
    private readonly IProductsRepository _repository = repository;

    public async Task<ProductResponse> ExecuteAsync(ProductRequest request, CancellationToken cancellationToken = default)
    {
        if(await _repository.ExistsAsync(request.Name, cancellationToken))
        {
            throw new DuplicateException("Product already exists.");
        }

        var product = Product.Create(request.Name, request.Quantity);

        await _repository.AddAsync(product, cancellationToken);

        return product;
    }
}
