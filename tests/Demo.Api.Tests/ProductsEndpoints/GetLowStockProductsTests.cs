using Bogus;
using Demo.Application.DTOs;
using System.Net.Http.Json;

namespace Demo.Api.Tests.ProductsEndpoints;

[Collection(nameof(CollectionIntegrationTests))]
public sealed class GetLowStockProductsTests
{
    private readonly IntegrationTestsFactory _factory;

    public GetLowStockProductsTests(IntegrationTestsFactory factory)
        => _factory = factory;


    [Fact]
    public async Task Should_Return_The_Created_Low_Stock_Product_and_200_StatusCode()
    {
        // Arrange
        var client = _factory.CreateClient();
        var product = new Faker<ProductRequest>()
            .RuleFor(p => p.Name, s => s.Commerce.ProductName())
            .RuleFor(p => p.Quantity, 1u)
            .Generate();

        await client.PostAsync(
            "/products",
            JsonContent.Create(product),
            CancellationToken.None);


        // Act
        var act = await client.GetAsync("/products/low-stock?threshold=5", CancellationToken.None);


        // Assert
        act.Should()
           .Be200Ok()
           .And.Satisfy<IEnumerable<ProductResponse>>(model =>
                model.Should().Contain(p => p.Name == product.Name && p.Quantity < 5));
    }
}
