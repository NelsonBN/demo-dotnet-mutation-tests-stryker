namespace Demo.Domain.Tests;

public sealed class ProductTest
{
    [Fact]
    public void When_Product_Is_Created_Then_It_Should_Have_A_Valid_Id_And_Fields_Filled()
    {
        // Arrange
        var name = "Product Name";
        var quantity = 10u;


        // Act
        var product = Product.Create(name, quantity);


        // Assert
        product.Id.Should().NotBeEmpty();
        product.Name.Should().Be(name);
        product.Quantity.Should().Be(quantity);
    }

    [Fact]
    public void When_Product_Is_Created_With_Empty_Name_Then_It_Should_Throw_InvalidException()
    {
        // Arrange
        var name = "";
        var quantity = 10u;


        // Act
        var act = () => Product.Create(name, quantity);


        // Assert
        act.Should().Throw<InvalidException>();
    }

    [Fact]
    public void When_Update_Is_Called_With_Empty_Name_Then_It_Should_Throw_InvalidException()
    {
        // Arrange
        var product = Product.Create("Product Name", 10);
        var name = "";
        var quantity = 10u;


        // Act
        var act = () => product.Update(name, quantity);


        // Assert
        act.Should().Throw<InvalidException>();
    }

    [Fact]
    public void When_Update_Is_Called_Then_It_Should_Update_The_Fields()
    {
        // Arrange
        var product = Product.Create("Product Name", 10);
        var name = "New Product Name";
        var quantity = 20u;


        // Act
        product.Update(name, quantity);


        // Assert
        product.Name.Should().Be(name);
        product.Quantity.Should().Be(quantity);
    }
}
