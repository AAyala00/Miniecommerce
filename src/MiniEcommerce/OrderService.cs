namespace MiniEcommerce;

public class OrderService(DiscountCalculator discountCalculator)
{
    private readonly List<Order> _orders = [];
    private int _nextOrderId = 1;

    public IReadOnlyList<Order> Orders => _orders;

    public bool IsEmpty => _orders.Count == 0;

    public Order? FindById(int orderId) => _orders.FirstOrDefault(o => o.Id == orderId);

    public Order CreateOrder(ShoppingCart cart, string customerName, string customerEmail, string? discountCode)
    {
        decimal subtotal = cart.Subtotal;
        decimal discount = discountCalculator.Calculate(subtotal, discountCode);
        decimal tax = TaxCalculator.Calculate(cart.Items);

        var order = new OrderBuilder()
            .WithId(_nextOrderId++)
            .WithItems(cart.GetItemsSnapshot())
            .WithCustomer(customerName, customerEmail)
            .WithDiscount(discount, discountCode)
            .WithTax(tax)
            .Build();

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