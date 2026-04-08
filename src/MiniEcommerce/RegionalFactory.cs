namespace MiniEcommerce;

// --- Abstract Factory ---
public interface IRegionalFactory
{
    ITaxStrategy CreateTaxStrategy();
    ICurrencyFormatter CreateCurrencyFormatter();
}

public interface ITaxStrategy
{
    decimal Calculate(IEnumerable<CartItem> items);
}

public interface ICurrencyFormatter
{
    string Format(decimal amount);
}

// --- Mexico Family ---
public class MexicoFactory : IRegionalFactory
{
    public ITaxStrategy CreateTaxStrategy() => new MexicoTaxStrategy();
    public ICurrencyFormatter CreateCurrencyFormatter() => new MexicoCurrencyFormatter();
}

public class MexicoTaxStrategy : ITaxStrategy
{
    private const decimal IvaRate = 0.16m;
    private static HashSet<string> Exempt = ["Food"];

    public decimal Calculate(IEnumerable<CartItem> items)
    {
        decimal taxable = items
            .Where(i => !Exempt.Contains(i.Product.Category))
            .Sum(i => i.Product.Price * i.Quantity);

        return taxable * IvaRate;
    }
}

public class MexicoCurrencyFormatter : ICurrencyFormatter
{
    public string Format(decimal amount) => $"${amount:N2} MXN";
}

// --- USA Family ---
public class UsaFactory : IRegionalFactory
{
    public ITaxStrategy CreateTaxStrategy() => new UsaTaxStrategy();
    public ICurrencyFormatter CreateCurrencyFormatter() => new UsaCurrencyFormatter();
}

public class UsaTaxStrategy : ITaxStrategy
{
    private const decimal SalesTaxRate = 0.08m; // 8% sales tax

    public decimal Calculate(IEnumerable<CartItem> items)
    {
        decimal total = items.Sum(i => i.Product.Price * i.Quantity);
        return total * SalesTaxRate;
    }
}

public class UsaCurrencyFormatter : ICurrencyFormatter
{
    public string Format(decimal amount) => $"${amount:N2} USD";
}