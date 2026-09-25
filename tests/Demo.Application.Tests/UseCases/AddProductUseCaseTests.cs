using Demo.Application.DTOs;
using Demo.Application.Services;
using Demo.Domain;
using Demo.UseCases;

namespace Demo.Application.Tests.UseCases;

public sealed class AddProductUseCaseTests
{
    private readonly IProductsRepository _repository;
    private readonly AddProductUseCase _useCase;

    public AddProductUseCaseTests()
    {
        _repository = Substitute.For<IProductsRepository>();
        _useCase = new(_repository);
    }


    [Fact]
    public async Task When_Product_Is_Created_Then_It_Should_Have_A_Valid_Id()
    {
        // Arrange
        var request = new ProductRequest { Name = "Product Name 1", Quantity = 10u };

        _repository.ExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                   .Returns(false);

        // Act
        var act = await _useCase.ExecuteAsync(request, CancellationToken.None);


        // Assert
        act.Id.Should().NotBeEmpty();
        act.Name.Should().Be(request.Name);
        act.Quantity.Should().Be(request.Quantity);


        await _repository
            .Received(1)
            .AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task When_Product_Is_Created_With_Existent_Name_Then_It_Should_Throw_DuplicateException()
    {
        // Arrange
        var request = new ProductRequest { Name = "Product Name 2", Quantity = 10u };

        _repository.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                   .Returns(true);


        // Act
        var act = () => _useCase.ExecuteAsync(request);


        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
    }
}
