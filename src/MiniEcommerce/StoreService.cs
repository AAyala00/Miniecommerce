namespace MiniEcommerce;

public class StoreService(
    ProductCatalog catalog,
    ShoppingCart cart,
    OrderService orderService,
    DiscountCalculator discountCalculator,
    IEnumerable<IOrdernotifier> notifiers)
{
    public void ShowProducts()
    {
        Console.WriteLine("\n=== PRODUCTOS DISPONIBLES ===");
        foreach (var p in catalog.Products)
        {
            Console.WriteLine($"  [{p.Id}] {p.Name} - ${p.Price:F2} ({p.Category}) - Stock: {p.Stock}");
        }
    }

    public void AddToCart(int productId, int quantity)
    {
        var product = catalog.FindById(productId);
        if (product == null)
        {
            Console.WriteLine("ERROR: Producto no encontrado.");
            return;
        }
        if (product.Stock < quantity)
        {
            Console.WriteLine($"ERROR: Stock insuficiente. Disponible: {product.Stock}");
            return;
        }

        cart.Add(product, quantity);
        Console.WriteLine($"OK: {quantity}x {product.Name} agregado al carrito.");
    }

    public void ShowCart()
    {
        if (cart.IsEmpty)
        {
            Console.WriteLine("\nEl carrito está vacío.");
            return;
        }

        Console.WriteLine("\n=== CARRITO ===");
        foreach (var item in cart.Items)
        {
            var lineTotal = item.Product.Price * item.Quantity;
            Console.WriteLine($"  {item.Quantity}x {item.Product.Name} - ${lineTotal:F2}");
        }
        Console.WriteLine($"  SUBTOTAL: ${cart.Subtotal:F2}");
    }

    public void RemoveFromCart(int productId)
    {
        if (!cart.Remove(productId))
            Console.WriteLine("ERROR: Producto no está en el carrito.");
        else
            Console.WriteLine("OK: Producto eliminado del carrito.");
    }

    public void Checkout(string customerName, string customerEmail, string? discountCode = null)
    {
        if (cart.IsEmpty)
        {
            Console.WriteLine("ERROR: El carrito está vacío.");
            return;
        }

        // --- Validaciones mezcladas ---
        if (string.IsNullOrWhiteSpace(customerName))
        {
            Console.WriteLine("ERROR: Nombre del cliente requerido.");
            return;
        }
        if (string.IsNullOrWhiteSpace(customerEmail) || !customerEmail.Contains('@'))
        {
            Console.WriteLine("ERROR: Email inválido.");
            return;
        }

        // --- Cálculo de descuento (lógica hardcodeada, Ya no) ---
        if (!discountCalculator.IsValidCode(discountCode))
            Console.WriteLine("WARN: Código de descuento inválido, se ignora.");

        var order = orderService.CreateOrder(cart, customerName, customerEmail, discountCode);

        // --- Imprimir recibo ---
        Console.WriteLine("\n=== ORDEN CONFIRMADA ===");
        Console.WriteLine($"  Orden #{order.Id}");
        Console.WriteLine($"  Cliente: {order.CustomerName} ({order.CustomerEmail})");
        foreach (var item in order.Items)
        {
            Console.WriteLine($"  {item.Quantity}x {item.Product.Name} - ${item.Product.Price * item.Quantity:F2}");
        }
        Console.WriteLine($"  Subtotal: ${order.Total - order.Tax + order.Discount:F2}");
        if (order.Discount > 0)
            Console.WriteLine($"  Descuento ({discountCode}): -${order.Discount:F2}");
        Console.WriteLine($"  IVA (16%): ${order.Tax:F2}");
        Console.WriteLine($"  TOTAL: ${order.Total:F2}");

        // --- Enviar notificación (simulada) ---
        foreach (var notifier in notifiers)
            notifier.OnOrderCreated(order);
    }

    public void ShowOrders()
    {
        if (orderService.IsEmpty)
        {
            Console.WriteLine("\nNo hay órdenes registradas.");
            return;
        }

        Console.WriteLine("\n=== ÓRDENES ===");
        foreach (var o in orderService.Orders)
        {
            Console.WriteLine($"  Orden #{o.Id} - {o.Status} - ${o.Total:F2} - {o.CustomerName} - {o.CreatedAt:g}");
        }
    }

    public void CancelOrder(int orderId)
    {
        var order = orderService.FindById(orderId);
        if (order == null)
        {
            Console.WriteLine("ERROR: Orden no encontrada.");
            return;
        }
        if (!OrderService.CancelOrder(order))
        {
            Console.WriteLine($"ERROR: No se puede cancelar una orden con estado '{order.Status}'.");
            return;
        }

        foreach (var notifier in notifiers)
            notifier.OnOrderCancelled(order);
    }
}