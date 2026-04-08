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
        // Simula descuentos que podrían venir de config/DB
        var discountDefinitions = new Dictionary<string, string>
        {
            ["WELCOME10"] = "percentage:0.10",
            ["SAVE20"] = "percentage:0.20",
            ["FLAT50"] = "flat:50",
            ["BUY2GET1"] = "BuyXGetY:2",
        };

        var calculator = new DiscountCalculator();

        foreach (var (code, definition) in discountDefinitions)
        {
            calculator.Register(DiscountFactory.Create(code, definition));
        }

        return calculator;
    }
}