using Demo.Application.Services;
using Demo.Application.UseCases;
using Demo.Domain;

namespace Demo.Application.Tests.UseCases;

public sealed class RestockProductUseCaseTests
{
    private readonly IProductsRepository _repository;
    private readonly RestockProductUseCase _useCase;

    public RestockProductUseCaseTests()
    {
        _repository = Substitute.For<IProductsRepository>();
        _useCase = new(_repository);
    }

    [Fact]
    public async Task When_Product_Exists_Then_It_Should_Increase_The_Quantity()
    {
        // Arrange
        var product = Product.Create("Product Name", 10);
        _repository.GetAsync(product.Id, Arg.Any<CancellationToken>())
                   .Returns(product);


        // Act
        var act = await _useCase.ExecuteAsync(product.Id, 5, CancellationToken.None);


        // Assert
        act.Quantity.Should().Be(15u);
        await _repository
            .Received(1)
            .UpdateAsync(Arg.Is<Product>(p => p.Quantity == 15u), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task When_Product_Not_Exists_Then_It_Should_Throw_NotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>())
                   .Returns((Product?)null);


        // Act
        var act = () => _useCase.ExecuteAsync(id, 5, CancellationToken.None);


        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        await _repository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }
}
