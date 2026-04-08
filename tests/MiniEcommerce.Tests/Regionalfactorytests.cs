using MiniEcommerce;

namespace MiniEcommerce.Tests;

public class RegionalFactoryTests
{
    private static List<CartItem> CreateItems(decimal price = 100m, string category = "Electronics") =>
    [
        new() { Product = new Product { Id = 1, Name = "Test", Price = price, Category = category, Stock = 10 }, Quantity = 1 }
    ];

    [Fact]
    public void MexicoFactory_Tax_Applies16PercentToNonFood()
    {
        IRegionalFactory factory = new MexicoFactory();
        var tax = factory.CreateTaxStrategy();

        Assert.Equal(16m, tax.Calculate(CreateItems(100m, "Electronics")));
    }

    [Fact]
    public void MexicoFactory_Tax_ExemptsFood()
    {
        IRegionalFactory factory = new MexicoFactory();
        var tax = factory.CreateTaxStrategy();

        Assert.Equal(0m, tax.Calculate(CreateItems(100m, "Food")));
    }

    [Fact]
    public void MexicoFactory_Currency_FormatsMXN()
    {
        IRegionalFactory factory = new MexicoFactory();
        var formatter = factory.CreateCurrencyFormatter();

        Assert.Equal("$100.00 MXN", formatter.Format(100m));
    }

    [Fact]
    public void UsaFactory_Tax_Applies8PercentToAll()
    {
        IRegionalFactory factory = new UsaFactory();
        var tax = factory.CreateTaxStrategy();

        Assert.Equal(8m, tax.Calculate(CreateItems(100m, "Food"))); // USA: food is taxed
    }

    [Fact]
    public void UsaFactory_Currency_FormatsUSD()
    {
        IRegionalFactory factory = new UsaFactory();
        var formatter = factory.CreateCurrencyFormatter();

        Assert.Equal("$100.00 USD", formatter.Format(100m));
    }

    [Fact]
    public void SameFactory_ProducesConsistentFamily()
    {
        // El punto del Abstract Factory: no puedes mezclar
        // IVA mexicano con formato USD por accidente
        IRegionalFactory mexico = new MexicoFactory();
        var tax = mexico.CreateTaxStrategy();
        var formatter = mexico.CreateCurrencyFormatter();

        var items = CreateItems(100m);
        var taxAmount = tax.Calculate(items);

        Assert.Equal("$16.00 MXN", formatter.Format(taxAmount));
    }
}