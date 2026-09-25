using Demo.Application.Services;
using Demo.Application.UseCases;
using Demo.Domain;

namespace Demo.Application.Tests.UseCases;

public sealed class GetLowStockProductsUseCaseTests
{
    private readonly IProductsRepository _repository;
    private readonly GetLowStockProductsUseCase _useCase;

    public GetLowStockProductsUseCaseTests()
    {
        _repository = Substitute.For<IProductsRepository>();
        _useCase = new(_repository);
    }

    [Fact]
    public async Task When_Some_Products_Are_Below_Threshold_Then_It_Should_Return_Only_Those()
    {
        // Arrange
        var products = new List<Product>() {
             Product.Create("Product 1", 2),
             Product.Create("Product 2", 20),
             Product.Create("Product 3", 5)
        };
        _repository.ListAsync(Arg.Any<CancellationToken>())
                   .Returns(products);


        // Act
        var act = await _useCase.ExecuteAsync(10, CancellationToken.None);

        // Assert
        act.Should().HaveCount(2)
           .And.OnlyContain(product => product.Quantity < 10);
    }

    [Fact]
    public async Task When_No_Product_Is_Below_Threshold_Then_It_Should_Return_Empty_Response()
    {
        // Arrange
        var products = new List<Product>() {
             Product.Create("Product 1", 20),
             Product.Create("Product 2", 30)
        };
        _repository.ListAsync(Arg.Any<CancellationToken>())
                   .Returns(products);


        // Act
        var act = await _useCase.ExecuteAsync(10, CancellationToken.None);

        // Assert
        act.Should().BeEmpty();
    }
}
