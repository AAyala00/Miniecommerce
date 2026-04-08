namespace MiniEcommerce;

public class ProductCatalog
{
    private readonly List<Product> _products = [];

    public IReadOnlyList<Product> Products => _products;

    public void Add(Product product) => _products.Add(product);

    public Product? FindById(int id) => _products.FirstOrDefault(p => p.Id == id);

    public static ProductCatalog CreateDefault()
    {
        var catalog = new ProductCatalog();
        catalog.Add(new Product { Id = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics", Stock = 10 });
        catalog.Add(new Product { Id = 2, Name = "Mouse", Price = 29.99m, Category = "Electronics", Stock = 50 });
        catalog.Add(new Product { Id = 3, Name = "T-Shirt", Price = 19.99m, Category = "Clothing", Stock = 100 });
        catalog.Add(new Product { Id = 4, Name = "Jeans", Price = 49.99m, Category = "Clothing", Stock = 30 });
        catalog.Add(new Product { Id = 5, Name = "Coffee Beans", Price = 14.99m, Category = "Food", Stock = 200 });
        return catalog;
    }
}