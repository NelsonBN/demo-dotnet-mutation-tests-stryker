using Bogus;
using Demo.Application.DTOs;
using System.Net.Http.Json;

namespace Demo.Api.Tests.ProductsEndpoints;

[Collection(nameof(CollectionIntegrationTests))]
public sealed class UpdateProductTests
{
    private readonly IntegrationTestsFactory _factory;

    public UpdateProductTests(IntegrationTestsFactory factory)
        => _factory = factory;


    [Fact]
    public async Task When_Product_Is_Updated_Then_It_Should_Return_204_StatusCode()
    {
        // Arrange
        var id = Guid.Parse("1c440211-66e0-4019-afea-0b0299ae599a");

        var product = new Faker<ProductRequest>()
            .RuleFor(p => p.Name, s => s.Commerce.ProductName())
            .RuleFor(p => p.Quantity, s => s.Random.UInt(1, 100))
            .Generate();


        // Act
        var act = await _factory.CreateClient()
            .PutAsync(
                $"/products/{id}",
                JsonContent.Create(product),
                CancellationToken.None);


        // Assert
        act.Should().Be204NoContent();
    }

    [Fact]
    public async Task When_Product_Is_Updated_With_Existent_Name_Then_It_Should_Return_409_StatusCode()
    {
        // Arrange
        var id = Guid.Parse("1c440211-66e0-4019-afea-0b0299ae599a");

        var product = new Faker<ProductRequest>()
            .RuleFor(p => p.Name, "Keyboard")
            .RuleFor(p => p.Quantity, s => s.Random.UInt(1, 100))
            .Generate();


        // Act
        var act = await _factory.CreateClient()
            .PutAsync(
                $"/products/{id}",
                JsonContent.Create(product),
                CancellationToken.None);


        // Assert
        act.Should().Be409Conflict();
    }

    [Fact]
    public async Task When_Product_Does_Not_Exist_Then_It_Should_Return_404_StatusCode()
    {
        // Arrange
        var id = Guid.NewGuid();

        var product = new Faker<ProductRequest>()
            .RuleFor(p => p.Name, s => s.Commerce.ProductName())
            .RuleFor(p => p.Quantity, s => s.Random.UInt(1, 100))
            .Generate();


        // Act
        var act = await _factory.CreateClient()
            .PutAsync(
                $"/products/{id}",
                JsonContent.Create(product),
                CancellationToken.None);


        // Assert
        act.Should().Be404NotFound();
    }
}
