using Bogus;
using Demo.Application.DTOs;
using System.Net.Http.Json;

namespace Demo.Api.Tests.ProductsEndpoints;

[Collection(nameof(CollectionIntegrationTests))]
public sealed class RestockProductTests
{
    private readonly IntegrationTestsFactory _factory;

    public RestockProductTests(IntegrationTestsFactory factory)
        => _factory = factory;


    [Fact]
    public async Task When_Product_Exists_Then_It_Should_Increase_The_Quantity_and_Return_200_StatusCode()
    {
        // Arrange
        var client = _factory.CreateClient();
        var product = new Faker<ProductRequest>()
            .RuleFor(p => p.Name, s => s.Commerce.ProductName())
            .RuleFor(p => p.Quantity, 10u)
            .Generate();

        var created = await client.PostAsync(
            "/products",
            JsonContent.Create(product),
            CancellationToken.None);
        var createdProduct = await created.Content.ReadFromJsonAsync<ProductResponse>(CancellationToken.None);


        // Act
        var act = await client.PostAsync(
            $"/products/{createdProduct!.Id}/restock?amount=5",
            content: null,
            CancellationToken.None);


        // Assert
        act.Should()
           .Be200Ok()
           .And.Satisfy<ProductResponse>(model => model.Quantity.Should().Be(15u));
    }

    [Fact]
    public async Task When_Product_Does_Not_Exist_Then_It_Should_Return_404_StatusCode()
    {
        // Arrange
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();


        // Act
        var act = await client.PostAsync(
            $"/products/{id}/restock?amount=5",
            content: null,
            CancellationToken.None);


        // Assert
        act.Should().Be404NotFound();
    }
}
