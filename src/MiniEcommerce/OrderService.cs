namespace MiniEcommerce;

public class OrderService
{
    private readonly List<Order> _orders = [];
    private int _nextOrderId = 1;

    public IReadOnlyList<Order> Orders => _orders;

    public bool IsEmpty => _orders.Count == 0;

    public Order? FindById(int orderId) => _orders.FirstOrDefault(o => o.Id == orderId);

    public Order CreateOrder(ShoppingCart cart, string customerName, string customerEmail, string? discountCode)
    {
        decimal subtotal = cart.Subtotal;
        decimal discount = DiscountCalculator.Calculate(subtotal, discountCode);
        decimal tax = TaxCalculator.Calculate(cart.Items);
        decimal total = subtotal - discount + tax;

        var order = new Order
        {
            Id = _nextOrderId++,
            Items = cart.GetItemsSnapshot(),
            Total = total,
            Discount = discount,
            Tax = tax,
            Status = "Confirmed",
            CustomerEmail = customerEmail,
            CustomerName = customerName,
            CreatedAt = DateTime.Now,
            DiscountCode = discountCode
        };

        // Actualizar stock
        foreach (var item in cart.Items)
        {
            item.Product.Stock -= item.Quantity;
        }

        _orders.Add(order);
        cart.Clear();

        return order;
    }

    public static bool CancelOrder(Order order)
    {
        if (order.Status is "Cancelled" or "Shipped")
            return false;

        order.Status = "Cancelled";

        foreach (var item in order.Items)
        {
            item.Product.Stock += item.Quantity;
        }

        return true;
    }
}