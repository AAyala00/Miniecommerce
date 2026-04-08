namespace MiniEcommerce;

public class DiscountCalculator
{
    private readonly Dictionary<string, IDiscountstrategy> _strategies = [];

    public DiscountCalculator Register(IDiscountstrategy strategy)
    {
        _strategies[strategy.Code] = strategy;
        return this;
    }

    public decimal Calculate(decimal subtotal, string? discountCode)
    {
        if (discountCode is null || !_strategies.TryGetValue(discountCode, out var strategy))
            return 0m;

        return strategy.Calculate(subtotal);
    }

    public bool IsValidCode(String? discountCode)
    {
        if (discountCode is null)
            return true; // null = no code, which is valid

        return _strategies.ContainsKey(discountCode);
    }

    public static DiscountCalculator CreateDefault()
    {
        var calculator = new DiscountCalculator();
        calculator.Register(new PercentageDiscount("WELCOME10", 0.10m));
        calculator.Register(new PercentageDiscount("SAVE20", 0.20m));
        calculator.Register(new FlatDiscount("FLAT50", 50m));
        calculator.Register(new BuyXGetYDiscount("BUY2GET1", 2));
        return calculator;
    }
}