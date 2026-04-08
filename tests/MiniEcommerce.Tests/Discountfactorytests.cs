using MiniEcommerce;

namespace MiniEcommerce.Tests;

public class Discountfactorytests
{
    [Fact]
    public void Create_PercentageType_ReturnsPercentageDiscount()
    {
        var strategy = DiscountFactory.Create("TEST", "percentage:0.15");

        Assert.Equal(15m, strategy.Calculate(100m));
    }

    [Fact]
    public void Create_FlatType_ReturnsFlatDiscount()
    {
        var strategy = DiscountFactory.Create("TEST", "flat:25");

        Assert.Equal(25m, strategy.Calculate(100m));
    }

    [Fact]
    public void Create_BuyXGetYType_ReturnsBuyXGetYDiscount()
    {
        var strategy = DiscountFactory.Create("TEST", "BuyXGetY:2");

        Assert.Equal(10m, strategy.Calculate(30m));
    }

    [Fact]
    public void Create_UnknownType_Throws()
    {
        Assert.Throws<ArgumentException>(() => DiscountFactory.Create("TEST", "bogof:2"));
    }
}