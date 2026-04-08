using MiniEcommerce;

namespace MiniEcommerce.Tests;

public class OrderBuilderTests
{
    private static List<CartItem> CreateItems(decimal price = 100m) =>
    [
        new() { Product = new Product { Id = 1, Name = "Test", Price = price, Category = "Electronics", Stock = 10 }, Quantity = 1 }
    ];

    [Fact]
    public void Build_WithAllFields_CreatesOrder()
    {
        var order = new OrderBuilder()
            .WithId(1)
            .WithItems(CreateItems(100m))
            .WithCustomer("Alan", "alan@test.com")
            .WithDiscount(10m, "WELCOME10")
            .WithTax(16m)
            .Build();

        Assert.Equal(1, order.Id);
        Assert.Equal("Alan", order.CustomerName);
        Assert.Equal(106m, order.Total); // 100 - 10 + 16
        Assert.Equal("Confirmed", order.Status);
    }

    [Fact]
    public void Build_WithoutItems_Throws()
    {
        var builder = new OrderBuilder()
            .WithId(1)
            .WithCustomer("Alan", "alan@test.com");

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void Build_WithoutCustomerName_Throws()
    {
        var builder = new OrderBuilder()
            .WithId(1)
            .WithItems(CreateItems());

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void Build_WithoutDiscount_DefaultsToZero()
    {
        var order = new OrderBuilder()
            .WithId(1)
            .WithItems(CreateItems(100m))
            .WithCustomer("Alan", "alan@test.com")
            .WithTax(16m)
            .Build();

        Assert.Equal(0m, order.Discount);
        Assert.Equal(116m, order.Total);
    }
}