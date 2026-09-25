using Demo.Application.Services;
using Demo.Application.UseCases;
using Demo.Domain;

namespace Demo.Application.Tests.UseCases;

public sealed class GetProductsUseCaseTests
{
    private readonly IProductsRepository _repository;
    private readonly GetProductsUseCase _useCase;

    public GetProductsUseCaseTests()
    {
        _repository = Substitute.For<IProductsRepository>();
        _useCase = new(_repository);
    }

    [Fact]
    public async Task When_Does_Not_Exist_Any_Product_Then_It_Should_Return_Empty_Response()
    {
        // Arrange
        var products = Enumerable.Empty<Product>();
        _repository.ListAsync(Arg.Any<CancellationToken>())
                   .Returns(products);


        // Act
        var act = await _useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        act.Should().BeEmpty();
    }

    [Fact]
    public async Task When_Products_Exists_Then_It_Should_Return_it()
    {
        // Arrange
        var products = new List<Product>() {
             Product.Create("Product 1", 10),
             Product.Create("Product 2", 20),
             Product.Create("Product 3", 30)
        };
        _repository.ListAsync(Arg.Any<CancellationToken>())
                   .Returns(products);


        // Act
        var act = await _useCase.ExecuteAsync(CancellationToken.None);


        // Assert
        act.Should().HaveCount(3);
    }
}
