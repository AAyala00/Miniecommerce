namespace MiniEcommerce;

public class TaxCalculator
{
    private const decimal TaxRate = 0.16m; // IVA Mexico
    private static readonly HashSet<string> TaxExemptCategories = ["Food"];

    public static decimal Calculate(IEnumerable<CartItem> items)
    {
        decimal taxableAmount = items
        .Where(item => !TaxExemptCategories.Contains(item.Product.Category))
        .Sum(item => item.Product.Price * item.Quantity);

        return taxableAmount * TaxRate;
    }
}