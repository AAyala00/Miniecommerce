using MiniEcommerce;

namespace MiniEcommerce.Tests;

public class ProductCatalogTests
{
    [Fact]
    public void FindById_ExistingProduct_ReturnsProduct()
    {
        var catalog = new ProductCatalog();
        catalog.Add(new Product
        {
            Id = 1,
            Name = "Laptop",
            Price = 100m,
            Category = "Electronics",
            Stock = 5
        });

        var result = catalog.FindById(1);

        Assert.NotNull(result);
        Assert.Equal("Laptop", result.Name);
    }

    [Fact]
    public void FindById_NonExistent_ReturnsNull()
    {
        var catalog = new ProductCatalog();

        Assert.Null(catalog.FindById(99));
    }

    [Fact]
    public void CreateDefault_Returns5Products()
    {
        var catalog = ProductCatalog.CreateDefault();

        Assert.Equal(5, catalog.Products.Count);
    }
}