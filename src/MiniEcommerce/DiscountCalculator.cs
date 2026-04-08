namespace MiniEcommerce;

public class DiscountCalculator
{
    public static decimal Calculate(decimal subtotal, string? discountCode)
    {
        if (discountCode is null)
            return 0m;

        return discountCode switch
        {
            "WELCOME10" => subtotal * 0.10m,
            "SAVE20" => subtotal * 0.20m,
            "FLAT50" => Math.Min(50m, subtotal),
            _ => 0m
        };
    }

    public static bool IsValidCode(String? discountCode)
    {
        if (discountCode is null)
            return true; // null = no code, which is valid

        return discountCode is "WELCOME10" or "SAVE20" or "FLAT50";
    }
}