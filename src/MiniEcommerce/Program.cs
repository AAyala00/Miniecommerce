// ============================================================
// VERSIÓN NAIVE — Todo funciona, pero todo está aquí.
// Este es el punto de partida que vamos a refactorizar
// aplicando SOLID y patrones de diseño progresivamente.
// ============================================================

// --- Entry Point ---

var store = new StoreService();

Console.WriteLine("=== MINI E-COMMERCE ===");
Console.WriteLine("Comandos: productos | agregar | carrito | quitar | checkout | ordenes | cancelar | salir");

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim().ToLower();

    switch (input)
    {
        case "productos":
            store.ShowProducts();
            break;

        case "agregar":
            Console.Write("  ID del producto: ");
            if (int.TryParse(Console.ReadLine(), out var pid))
            {
                Console.Write("  Cantidad: ");
                if (int.TryParse(Console.ReadLine(), out var qty))
                    store.AddToCart(pid, qty);
            }
            break;

        case "carrito":
            store.ShowCart();
            break;

        case "quitar":
            Console.Write("  ID del producto a quitar: ");
            if (int.TryParse(Console.ReadLine(), out var rid))
                store.RemoveFromCart(rid);
            break;

        case "checkout":
            Console.Write("  Tu nombre: ");
            var name = Console.ReadLine() ?? "";
            Console.Write("  Tu email: ");
            var email = Console.ReadLine() ?? "";
            Console.Write("  Código de descuento (enter para omitir): ");
            var code = Console.ReadLine();
            store.Checkout(name, email, string.IsNullOrWhiteSpace(code) ? null : code);
            break;

        case "ordenes":
            store.ShowOrders();
            break;

        case "cancelar":
            Console.Write("  ID de la orden: ");
            if (int.TryParse(Console.ReadLine(), out var oid))
                store.CancelOrder(oid);
            break;

        case "salir":
            Console.WriteLine("¡Hasta luego!");
            return;

        default:
            Console.WriteLine("Comando no reconocido.");
            break;
    }
}

// --- Modelos ---

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public string Category { get; set; } = ""; // "Electronics", "Clothing", "Food"
    public int Stock { get; set; }
}

public class CartItem
{
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public List<CartItem> Items { get; set; } = [];
    public decimal Total { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public string Status { get; set; } = "Pending"; // "Pending", "Confirmed", "Shipped", "Cancelled"
    public string CustomerEmail { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public string? DiscountCode { get; set; }
}

// --- La clase que hace TODO ---

public class StoreService
{
    private readonly List<Product> _products = [];
    private readonly List<CartItem> _cart = [];
    private readonly List<Order> _orders = [];
    private int _nextOrderId = 1;

    public StoreService()
    {
        // Seed de productos hardcodeado
        _products.Add(new Product { Id = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics", Stock = 10 });
        _products.Add(new Product { Id = 2, Name = "Mouse", Price = 29.99m, Category = "Electronics", Stock = 50 });
        _products.Add(new Product { Id = 3, Name = "T-Shirt", Price = 19.99m, Category = "Clothing", Stock = 100 });
        _products.Add(new Product { Id = 4, Name = "Jeans", Price = 49.99m, Category = "Clothing", Stock = 30 });
        _products.Add(new Product { Id = 5, Name = "Coffee Beans", Price = 14.99m, Category = "Food", Stock = 200 });
    }

    public void ShowProducts()
    {
        Console.WriteLine("\n=== PRODUCTOS DISPONIBLES ===");
        foreach (var p in _products)
        {
            Console.WriteLine($"  [{p.Id}] {p.Name} - ${p.Price:F2} ({p.Category}) - Stock: {p.Stock}");
        }
    }

    public void AddToCart(int productId, int quantity)
    {
        var product = _products.FirstOrDefault(p => p.Id == productId);
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

        var existing = _cart.FirstOrDefault(c => c.Product.Id == productId);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            _cart.Add(new CartItem { Product = product, Quantity = quantity });
        }

        Console.WriteLine($"OK: {quantity}x {product.Name} agregado al carrito.");
    }

    public void ShowCart()
    {
        if (_cart.Count == 0)
        {
            Console.WriteLine("\nEl carrito está vacío.");
            return;
        }

        Console.WriteLine("\n=== CARRITO ===");
        decimal subtotal = 0;
        foreach (var item in _cart)
        {
            var lineTotal = item.Product.Price * item.Quantity;
            subtotal += lineTotal;
            Console.WriteLine($"  {item.Quantity}x {item.Product.Name} - ${lineTotal:F2}");
        }
        Console.WriteLine($"  SUBTOTAL: ${subtotal:F2}");
    }

    public void RemoveFromCart(int productId)
    {
        var item = _cart.FirstOrDefault(c => c.Product.Id == productId);
        if (item == null)
        {
            Console.WriteLine("ERROR: Producto no está en el carrito.");
            return;
        }
        _cart.Remove(item);
        Console.WriteLine($"OK: {item.Product.Name} eliminado del carrito.");
    }

    public void Checkout(string customerName, string customerEmail, string? discountCode = null)
    {
        if (_cart.Count == 0)
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

        // --- Cálculo de subtotal ---
        decimal subtotal = 0;
        foreach (var item in _cart)
        {
            subtotal += item.Product.Price * item.Quantity;
        }

        // --- Cálculo de descuento (lógica hardcodeada) ---
        decimal discount = 0;
        if (discountCode == "WELCOME10")
        {
            discount = subtotal * 0.10m;
        }
        else if (discountCode == "SAVE20")
        {
            discount = subtotal * 0.20m;
        }
        else if (discountCode == "FLAT50")
        {
            discount = Math.Min(50m, subtotal);
        }
        else if (discountCode != null)
        {
            Console.WriteLine("WARN: Código de descuento inválido, se ignora.");
        }

        // --- Cálculo de impuesto (hardcodeado) ---
        decimal taxRate = 0.16m; // IVA México
        // Pero si es comida, el IVA es 0%
        decimal taxableAmount = 0;
        foreach (var item in _cart)
        {
            if (item.Product.Category != "Food")
            {
                taxableAmount += item.Product.Price * item.Quantity;
            }
        }
        decimal tax = taxableAmount * taxRate;

        decimal total = subtotal - discount + tax;

        // --- Crear la orden ---
        var order = new Order
        {
            Id = _nextOrderId++,
            Items = new List<CartItem>(_cart),
            Total = total,
            Discount = discount,
            Tax = tax,
            Status = "Confirmed",
            CustomerEmail = customerEmail,
            CustomerName = customerName,
            CreatedAt = DateTime.Now,
            DiscountCode = discountCode
        };

        // --- Actualizar stock ---
        foreach (var item in _cart)
        {
            item.Product.Stock -= item.Quantity;
        }

        _orders.Add(order);
        _cart.Clear();

        // --- Imprimir recibo ---
        Console.WriteLine("\n=== ORDEN CONFIRMADA ===");
        Console.WriteLine($"  Orden #{order.Id}");
        Console.WriteLine($"  Cliente: {order.CustomerName} ({order.CustomerEmail})");
        foreach (var item in order.Items)
        {
            Console.WriteLine($"  {item.Quantity}x {item.Product.Name} - ${item.Product.Price * item.Quantity:F2}");
        }
        Console.WriteLine($"  Subtotal: ${subtotal:F2}");
        if (discount > 0)
            Console.WriteLine($"  Descuento ({discountCode}): -${discount:F2}");
        Console.WriteLine($"  IVA (16%): ${tax:F2}");
        Console.WriteLine($"  TOTAL: ${total:F2}");

        // --- Enviar notificación (simulada) ---
        Console.WriteLine($"\n  [EMAIL] Enviando confirmación a {customerEmail}...");
        Console.WriteLine($"  [LOG] Orden #{order.Id} creada por ${total:F2}");
    }

    public void ShowOrders()
    {
        if (_orders.Count == 0)
        {
            Console.WriteLine("\nNo hay órdenes registradas.");
            return;
        }

        Console.WriteLine("\n=== ÓRDENES ===");
        foreach (var o in _orders)
        {
            Console.WriteLine($"  Orden #{o.Id} - {o.Status} - ${o.Total:F2} - {o.CustomerName} - {o.CreatedAt:g}");
        }
    }

    public void CancelOrder(int orderId)
    {
        var order = _orders.FirstOrDefault(o => o.Id == orderId);
        if (order == null)
        {
            Console.WriteLine("ERROR: Orden no encontrada.");
            return;
        }
        if (order.Status == "Cancelled")
        {
            Console.WriteLine("ERROR: La orden ya está cancelada.");
            return;
        }
        if (order.Status == "Shipped")
        {
            Console.WriteLine("ERROR: No se puede cancelar una orden enviada.");
            return;
        }

        order.Status = "Cancelled";

        // Restaurar stock
        foreach (var item in order.Items)
        {
            item.Product.Stock += item.Quantity;
        }

        Console.WriteLine($"OK: Orden #{orderId} cancelada.");
        Console.WriteLine($"  [EMAIL] Enviando notificación de cancelación a {order.CustomerEmail}...");
    }
}