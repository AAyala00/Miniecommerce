namespace MiniEcommerce;

public interface IDiscountstrategy
{
    string Code { get; }
    decimal Calculate(decimal subtotal);
}