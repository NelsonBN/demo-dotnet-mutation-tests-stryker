using Demo.Application.DTOs;

namespace Demo.Api.Tests.ProductsEndpoints;

[Collection(nameof(CollectionIntegrationTests))]
public sealed class GetProductTests
{
    private readonly IntegrationTestsFactory _factory;

    public GetProductTests(IntegrationTestsFactory factory)
        => _factory = factory;


    [Fact]
    public async Task When_Product_Not_Found_Then_It_Should_Return_404_StatusCode()
    {
        // Arrange
        var id = Guid.NewGuid();


        // Act
        var act = await _factory.CreateClient()
            .GetAsync($"/products/{id}", CancellationToken.None);


        // Assert
        act.Should().Be404NotFound();
    }

    [Fact]
    public async Task When_Product_Exists_Then_It_Should_Return_It_With_200_StatusCode()
    {
        // Arrange
        var id = Guid.Parse("412548eb-f19b-4049-be67-d7fcf4d19461");


        // Act
        var act = await _factory.CreateClient()
            .GetAsync($"/products/{id}", CancellationToken.None);


        // Assert
        act.Should()
           .Be200Ok()
           .And.Satisfy<ProductResponse>(model =>
                model.Should().Match<ProductResponse>(m =>
                    m.Id == id &&
                    m.Name == "Motherboard" &&
                    m.Quantity == 23));
    }
}
