using MiniEcommerce;

namespace MiniEcommerce.Tests;

public class DiscountCalculatorTests
{
    [Fact]
    public void Calculate_WithWelcome10_Returns10Percent()
    {
        var result = DiscountCalculator.Calculate(100m, "WELCOME10");

        Assert.Equal(10m, result);
    }

    [Fact]
    public void Calculate_WithFlat50_CapsAtSubtotal()
    {
        var result = DiscountCalculator.Calculate(30m, "FLAT50");

        Assert.Equal(30m, result); // no puede descontar más que el subtotal
    }

    [Fact]
    public void Calculate_WithNullCode_ReturnsZero()
    {
        var result = DiscountCalculator.Calculate(100m, null);

        Assert.Equal(0m, result);
    }

    [Fact]
    public void Calculate_WithInvalidCode_ReturnsZero()
    {
        var result = DiscountCalculator.Calculate(100m, "FAKE");

        Assert.Equal(0m, result);
    }

    [Theory]
    [InlineData("WELCOME10", true)]
    [InlineData("SAVE20", true)]
    [InlineData("FLAT50", true)]
    [InlineData("FAKE", false)]
    [InlineData(null, true)]
    public void IsValidCode_ReturnsExpected(string? code, bool expected)
    {
        Assert.Equal(expected, DiscountCalculator.IsValidCode(code));
    }
}

public class TaxCalculatorTests
{
    [Fact]
    public void Calculate_ElectronicsItem_Applies16PercentTax()
    {
        var items = new List<CartItem>
        {
          new() {Product = new Product {Price = 100m, Category = "Electronics",}, Quantity = 1}
        };

        Assert.Equal(16m, TaxCalculator.Calculate(items));
    }

    [Fact]
    public void Calculate_FoodItem_IsExempt()
    {
        var items = new List<CartItem>
        {
          new() {Product = new Product {Price = 100m, Category = "Food",}, Quantity = 1}
        };

        Assert.Equal(0m, TaxCalculator.Calculate(items));
    }

    [Fact]
    public void Calculate_MixedCart_TaxesOnlyNonFoodItems()
    {
        var items = new List<CartItem>
        {
            new() { Product = new Product { Price = 100m, Category = "Electronics" }, Quantity = 1 },
            new() { Product = new Product { Price = 50m, Category = "Food" }, Quantity = 2 }
        };

        Assert.Equal(16m, TaxCalculator.Calculate(items)); // solo los 100 de electronics
    }
}