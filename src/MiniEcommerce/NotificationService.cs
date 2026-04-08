namespace MiniEcommerce;

public class EmailNotifier : IOrdernotifier
{
    public void OnOrderCreated(Order order) => Console.WriteLine($"  [EMAIL] Enviando confirmación a {order.CustomerEmail}...");

    public void OnOrderCancelled(Order order) => Console.WriteLine($"  [EMAIL] Enviando cancelación a {order.CustomerEmail}...");
}

public class LogNotifier : IOrdernotifier
{
    public void OnOrderCreated(Order order) => Console.WriteLine($"  [LOG] Orden #{order.Id} creada por ${order.Total:F2}");

    public void OnOrderCancelled(Order order) => Console.WriteLine($"  [LOG] Orden #{order.Id} cancelada");

}