namespace Demo.Api.Tests.ProductsEndpoints;

[Collection(nameof(CollectionIntegrationTests))]
public sealed class DeleteProductTests
{
    private readonly IntegrationTestsFactory _factory;

    public DeleteProductTests(IntegrationTestsFactory factory)
        => _factory = factory;

    [Fact]
    public async Task When_Product_Exists_Should_Should_Return_204_StatusCode()
    {
        // Arrange
        var id = "31e88e92-b534-4121-a201-8027d805f86c";


        // Act
        var act = await _factory.CreateClient()
            .DeleteAsync($"/products/{id}", CancellationToken.None);


        // Assert
        act.Should().Be204NoContent();
    }

    [Fact]
    public async Task When_Product_Not_Exists_Should_Return_404_StatusCode()
    {
        // Arrange
        var id = Guid.NewGuid();


        // Act
        var act = await _factory.CreateClient()
            .DeleteAsync($"/products/{id}", CancellationToken.None);


        // Assert
        act.Should().Be404NotFound();
    }
}
