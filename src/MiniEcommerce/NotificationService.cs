namespace MiniEcommerce;

public class NotificationService
{
    public static void SendOrderConfirmation(Order order)
    {
        Console.WriteLine($"\n  [EMAIL] Enviando confirmación a {order.CustomerEmail}...");
        Console.WriteLine($"  [LOG] Orden #{order.Id} creada por ${order.Total:F2}");
    }

    public static void SendOrderCancellation(Order order)
    {
        Console.WriteLine($"  [EMAIL] Enviando notificación de cancelación a {order.CustomerEmail}...");
    }
}