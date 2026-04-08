using MiniEcommerce;

namespace MiniEcommerce.Tests;

public class OrderServiceTests
{
    private readonly OrderService _sut = new(DiscountCalculator.CreateDefault());

    private static ShoppingCart CreateCartWithProduct(decimal price = 100m, string category = "Electronics", int quantity = 1, int stock = 10)
    {
        var cart = new ShoppingCart();
        cart.Add(new Product { Id = 1, Name = "Test", Price = price, Category = category, Stock = stock }, quantity);
        return cart;
    }

    [Fact]
    public void CreateOrder_ReturnsConfirmedOrder()
    {
        var cart = CreateCartWithProduct(price: 100m);

        var order = _sut.CreateOrder(cart, "Alan", "alan@test.com", null);

        Assert.Equal("Confirmed", order.Status);
        Assert.Equal("Alan", order.CustomerName);
    }

    [Fact]
    public void CreateOrder_CalculatesTotalWithTax()
    {
        var cart = CreateCartWithProduct(price: 100m, category: "Electronics");

        var order = _sut.CreateOrder(cart, "Alan", "alan@test.com", null);

        Assert.Equal(116m, order.Total); // 100 + 16% IVA
    }

    [Fact]
    public void CreateOrder_AppliesDiscount()
    {
        var cart = CreateCartWithProduct(price: 100m, category: "Electronics");

        var order = _sut.CreateOrder(cart, "Alan", "alan@test.com", "WELCOME10");

        Assert.Equal(10m, order.Discount);
    }

    [Fact]
    public void CreateOrder_ReducesStock()
    {
        var product = new Product { Id = 1, Name = "Test", Price = 50m, Category = "Electronics", Stock = 10 };
        var cart = new ShoppingCart();
        cart.Add(product, 3);

        _sut.CreateOrder(cart, "Alan", "alan@test.com", null);

        Assert.Equal(7, product.Stock);
    }

    [Fact]
    public void CreateOrder_ClearsCart()
    {
        var cart = CreateCartWithProduct();

        _sut.CreateOrder(cart, "Alan", "alan@test.com", null);

        Assert.True(cart.IsEmpty);
    }

    [Fact]
    public void CancelOrder_ConfirmedOrder_ReturnsTrue()
    {
        var cart = CreateCartWithProduct();
        var order = _sut.CreateOrder(cart, "Alan", "alan@test.com", null);

        Assert.True(OrderService.CancelOrder(order));
        Assert.Equal("Cancelled", order.Status);
    }

    [Fact]
    public void CancelOrder_ShippedOrder_ReturnsFalse()
    {
        var cart = CreateCartWithProduct();
        var order = _sut.CreateOrder(cart, "Alan", "alan@test.com", null);
        order.Status = "Shipped";

        Assert.False(OrderService.CancelOrder(order));
    }

    [Fact]
    public void CancelOrder_RestoresStock()
    {
        var product = new Product { Id = 1, Name = "Test", Price = 50m, Category = "Electronics", Stock = 10 };
        var cart = new ShoppingCart();
        cart.Add(product, 3);

        var order = _sut.CreateOrder(cart, "Alan", "alan@test.com", null);
        OrderService.CancelOrder(order);

        Assert.Equal(10, product.Stock);
    }
}