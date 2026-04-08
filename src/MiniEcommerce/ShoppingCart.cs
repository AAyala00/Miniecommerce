namespace MiniEcommerce;

public class ShoppingCart
{
    private readonly List<CartItem> _items = [];

    public IReadOnlyList<CartItem> Items => _items;
    public bool IsEmpty => _items.Count == 0;

    public decimal Subtotal => _items.Sum(item => item.Product.Price * item.Quantity);

    public void Add(Product product, int quantity)
    {
        var existing = _items.FirstOrDefault(c => c.Product.Id == product.Id);
        if (existing != null)
            existing.Quantity += quantity;
        else
            _items.Add(new CartItem { Product = product, Quantity = quantity });
    }

    public bool Remove(int productId)
    {
        var item = _items.FirstOrDefault(c => c.Product.Id == productId);
        if (item == null)
            return false;

        _items.Remove(item);
        return true;
    }

    public List<CartItem> GetItemsSnapshot() => [.. _items];
    public void Clear() => _items.Clear();
}

