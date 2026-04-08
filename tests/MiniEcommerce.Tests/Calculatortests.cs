using MiniEcommerce;

namespace MiniEcommerce.Tests;

public class DiscountCalculatorTests
{
    private readonly DiscountCalculator _sut = DiscountCalculator.CreateDefault();

    [Fact]
    public void Calculate_WithWelcome10_Returns10Percent()
    {
        Assert.Equal(10m, _sut.Calculate(100m, "WELCOME10"));
    }

    [Fact]
    public void Calculate_WithFlat50_CapsAtSubtotal()
    {
        Assert.Equal(30m, _sut.Calculate(30m, "FLAT50")); // no puede descontar más que el subtotal
    }

    [Fact]
    public void Calculate_WithNullCode_ReturnsZero()
    {
        Assert.Equal(0m, _sut.Calculate(100m, null));
    }

    [Fact]
    public void Calculate_WithInvalidCode_ReturnsZero()
    {
        Assert.Equal(0m, _sut.Calculate(100m, "FAKE"));
    }

    [Theory]
    [InlineData("WELCOME10", true)]
    [InlineData("SAVE20", true)]
    [InlineData("FLAT50", true)]
    [InlineData("BUY2GET1", true)]
    [InlineData("FAKE", false)]
    [InlineData(null, true)]
    public void IsValidCode_ReturnsExpected(string? code, bool expected)
    {
        Assert.Equal(expected, _sut.IsValidCode(code));
    }

    [Fact]
    public void Register_NewStrategy_CanBeUsed()
    {
        var calculator = new DiscountCalculator();
        calculator.Register(new PercentageDiscount("VIP50", 0.50m));

        Assert.Equal(50m, calculator.Calculate(100m, "VIP50"));
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