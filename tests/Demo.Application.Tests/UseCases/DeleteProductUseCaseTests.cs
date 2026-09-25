using Demo.Application.Services;
using Demo.Application.UseCases;
using Demo.Domain;

namespace Demo.Application.Tests.UseCases;

public sealed class DeleteProductUseCaseTests
{
    private readonly IProductsRepository _repository;
    private readonly DeleteProductUseCase _useCase;

    public DeleteProductUseCaseTests()
    {
        _repository = Substitute.For<IProductsRepository>();
        _useCase = new(_repository);
    }


    [Fact]
    public async Task When_Product_Exists_Should_Be_Deleted()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repository.ExistsAsync(id, Arg.Any<CancellationToken>())
                   .Returns(true);


        // Act
        await _useCase.ExecuteAsync(id, CancellationToken.None);


        // Assert
        await _repository
            .Received(1)
            .DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task When_Product_Not_Exists_Should_Trow_NotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repository.ExistsAsync(id, Arg.Any<CancellationToken>())
                   .Returns(false);

        // Act
        var result = () => _useCase.ExecuteAsync(id, CancellationToken.None);


        // Assert
        await result.Should().ThrowAsync<NotFoundException>();
    }
}
