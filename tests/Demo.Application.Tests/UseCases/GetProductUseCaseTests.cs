using Demo.Application.Services;
using Demo.Application.UseCases;
using Demo.Domain;

namespace Demo.Application.Tests.UseCases;

public sealed class GetProductUseCaseTests
{
    private readonly IProductsRepository _repository;
    private readonly GetProductUseCase _useCase;

    public GetProductUseCaseTests()
    {
        _repository = Substitute.For<IProductsRepository>();
        _useCase = new(_repository);
    }

    [Fact]
    public async Task When_Product_Not_Found_Then_It_Should_Throw_NotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repository.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                   .Returns((Product?)null);


        // Act
        var act = async () => await _useCase.ExecuteAsync(id, CancellationToken.None);


        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task When_Product_Exists_Then_It_Should_Return_It()
    {
        // Arrange
        var product = Product.Create("Product 1", 10);

        _repository.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                   .Returns(product);


        // Act
        var act = await _useCase.ExecuteAsync(product.Id, CancellationToken.None);


        // Assert
        act.Id.Should().Be(product.Id);
        act.Name.Should().Be(product.Name);
        act.Quantity.Should().Be(product.Quantity);
    }
}
