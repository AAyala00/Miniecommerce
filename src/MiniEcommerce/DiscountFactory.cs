namespace MiniEcommerce;

public class DiscountFactory
{
    public static IDiscountstrategy Create(string code, string definition)
    {
        var parts = definition.Split(':');
        var type = parts[0];
        var value = decimal.Parse(parts[1]);

        return type switch
        {
            "percentage" => new PercentageDiscount(code, value),
            "flat" => new FlatDiscount(code, value),
            "BuyXGetY" => new BuyXGetYDiscount(code, (int)value),
            _ => throw new ArgumentException($"Tipo de descuento desconocido: {type}")
        };
    }
}