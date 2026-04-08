namespace MiniEcommerce;

public class OrderBuilder
{
    private int _id;
    private List<CartItem> _items = [];
    private string _customerName = string.Empty;
    private string _customerEmail = string.Empty;
    private string? _discountCode;
    private decimal _discount;
    private decimal _tax;

    public OrderBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public OrderBuilder WithItems(List<CartItem> items)
    {
        _items = items;
        return this;
    }

    public OrderBuilder WithCustomer(string name, string email)
    {
        _customerName = name;
        _customerEmail = email;
        return this;
    }

    public OrderBuilder WithDiscount(decimal discount, string? code)
    {
        _discount = discount;
        _discountCode = code;
        return this;
    }

    public OrderBuilder WithTax(decimal tax)
    {
        _tax = tax;
        return this;
    }

    public Order Build()
    {
        if (_items.Count == 0)
            throw new InvalidOperationException("La orden debe tener al menos un item.");
        if (string.IsNullOrWhiteSpace(_customerName))
            throw new InvalidOperationException("El nombre del cliente es requerido");
        if (string.IsNullOrWhiteSpace(_customerEmail))
            throw new InvalidOperationException("El email del cliente es requerido.");

        decimal subtotal = _items.Sum(i => i.Product.Price * i.Quantity);
        decimal total = subtotal - _discount + _tax;

        return new Order
        {
            Id = _id,
            Items = _items,
            Total = total,
            Discount = _discount,
            Tax = _tax,
            Status = "Confirmed",
            CustomerEmail = _customerEmail,
            CustomerName = _customerName,
            CreatedAt = DateTime.Now,
            DiscountCode = _discountCode
        };
    }
}