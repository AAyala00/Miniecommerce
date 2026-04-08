using MiniEcommerce;

namespace MiniEcommerce.Tests;

public class ShoppingCartTests
{
    private readonly ShoppingCart _sut = new();

    private static Product CreateProduct(int id = 1, decimal price = 100m) => new() { Id = id, Name = "Test", Price = price, Category = "Electronics", Stock = 10 };

    [Fact]
    public void Add_NewProduct_AppearsInItems()
    {
        _sut.Add(CreateProduct(), 2);

        Assert.Single(_sut.Items);
        Assert.Equal(2, _sut.Items[0].Quantity);
    }

    [Fact]
    public void Add_SameProductTwice_IncrementsQuantity()
    {
        var product = CreateProduct();

        _sut.Add(product, 1);
        _sut.Add(product, 3);

        Assert.Single(_sut.Items);
        Assert.Equal(4, _sut.Items[0].Quantity);
    }

    [Fact]
    public void Remove_ExistingProduct_ReturnsTrue()
    {
        _sut.Add(CreateProduct(id: 1), 1);

        Assert.True(_sut.Remove(1));
        Assert.True(_sut.IsEmpty);
    }

    [Fact]
    public void Remove_NonExistentProduct_ReturnsFalse()
    {
        Assert.False(_sut.Remove(99));
    }

    [Fact]
    public void Subtotal_CalculatesCorrectly()
    {
        _sut.Add(CreateProduct(id: 1, price: 50m), 2); // 100
        _sut.Add(CreateProduct(id: 2, price: 30m), 1); // 30

        Assert.Equal(130m, _sut.Subtotal);
    }

    [Fact]
    public void Clear_EmptiesCart()
    {
        _sut.Add(CreateProduct(), 1);
        _sut.Clear();

        Assert.True(_sut.IsEmpty);
    }
}