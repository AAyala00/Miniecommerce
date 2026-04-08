namespace MiniEcommerce;

public class PercentageDiscount(string code, decimal percentage) : IDiscountstrategy
{
    public string Code => code;

    public decimal Calculate(decimal subtotal) => subtotal * percentage;
}

public class FlatDiscount(string code, decimal amount) : IDiscountstrategy
{
    public string Code => code;

    public decimal Calculate(decimal subtotal) => Math.Min(amount, subtotal);
}

public class BuyXGetYDiscount(string code, int buyCount) : IDiscountstrategy
{
    public string Code => code;

    public decimal Calculate(decimal subtotal) => subtotal / (buyCount + 1);
}