namespace MiniEcommerce;

public class Order
{
    public int Id { get; set; }
    public List<CartItem> Items { get; set; } = [];
    public decimal Total { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public string Status { get; set; } = "Pending"; // "Pending", "Confirmed", "Shipped", "Cancelled"
    public string CustomerEmail { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public string? DiscountCode { get; set; }
}