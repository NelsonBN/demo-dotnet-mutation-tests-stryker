using Bogus;
using Demo.Application.DTOs;
using System.Net.Http.Json;

namespace Demo.Api.Tests.ProductsEndpoints;

[Collection(nameof(CollectionIntegrationTests))]
public sealed class AddProductTests
{
    private readonly IntegrationTestsFactory _factory;

    public AddProductTests(IntegrationTestsFactory factory)
        => _factory = factory;


    [Fact]
    public async Task When_Product_Is_Created_Then_It_Should_Return_201_StatusCode()
    {
        // Arrange
        var product = new Faker<ProductRequest>()
            .RuleFor(p => p.Name, s => s.Commerce.ProductName())
            .RuleFor(p => p.Quantity, s => s.Random.UInt(1, 100))
            .Generate();


       // Act
       var act = await _factory.CreateClient()
           .PostAsync(
               "/products",
               JsonContent.Create(product),
               CancellationToken.None);


        // Assert
        act.Should()
           .Be201Created()
           .And.Satisfy<ProductResponse>(model =>
           {
                model.Id.Should().NotBeEmpty();
                model.Name.Should().Be(product.Name);
                model.Quantity.Should().Be(product.Quantity);
           });
    }

    [Fact]
    public async Task When_Product_Is_Created_With_Existent_Name_Then_It_Should_Return_409_StatusCode()
    {
        // Arrange
        var product = new Faker<ProductRequest>()
            .RuleFor(p => p.Name, "Motherboard")
            .RuleFor(p => p.Quantity, s => s.Random.UInt(1, 100))
            .Generate();


        // Act
        var act = await _factory.CreateClient()
            .PostAsync(
                "/products",
                JsonContent.Create(product),
               CancellationToken.None);


        // Assert
        act.Should().Be409Conflict();
    }
}
