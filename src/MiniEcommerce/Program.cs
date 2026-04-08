// ============================================================
// VERSIÓN NAIVE — Todo funciona, pero todo está aquí.
// Este es el punto de partida que vamos a refactorizar
// aplicando SOLID y patrones de diseño progresivamente.
// ============================================================

// --- Entry Point ---
using MiniEcommerce;

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
namespace MiniEcommerce // definimos un namespace para no dejar las clases en top-level statements y que discount y tax calculator puedan ver las clases
{

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
        private readonly ProductCatalog _catalog = ProductCatalog.CreateDefault();
        private readonly ShoppingCart _cart = new();
        private readonly OrderService _orderService = new();

        public void ShowProducts()
        {
            Console.WriteLine("\n=== PRODUCTOS DISPONIBLES ===");
            foreach (var p in _catalog.Products)
            {
                Console.WriteLine($"  [{p.Id}] {p.Name} - ${p.Price:F2} ({p.Category}) - Stock: {p.Stock}");
            }
        }

        public void AddToCart(int productId, int quantity)
        {
            var product = _catalog.FindById(productId);
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

            _cart.Add(product, quantity);
            Console.WriteLine($"OK: {quantity}x {product.Name} agregado al carrito.");
        }

        public void ShowCart()
        {
            if (_cart.IsEmpty)
            {
                Console.WriteLine("\nEl carrito está vacío.");
                return;
            }

            Console.WriteLine("\n=== CARRITO ===");
            foreach (var item in _cart.Items)
            {
                var lineTotal = item.Product.Price * item.Quantity;
                Console.WriteLine($"  {item.Quantity}x {item.Product.Name} - ${lineTotal:F2}");
            }
            Console.WriteLine($"  SUBTOTAL: ${_cart.Subtotal:F2}");
        }

        public void RemoveFromCart(int productId)
        {
            if (!_cart.Remove(productId))
                Console.WriteLine("ERROR: Producto no está en el carrito.");
            else
                Console.WriteLine("OK: Producto eliminado del carrito.");
        }

        public void Checkout(string customerName, string customerEmail, string? discountCode = null)
        {
            if (_cart.IsEmpty)
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
            if (!DiscountCalculator.IsValidCode(discountCode))
                Console.WriteLine("WARN: Código de descuento inválido, se ignora.");

            var order = _orderService.CreateOrder(_cart, customerName, customerEmail, discountCode);

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
            NotificationService.SendOrderConfirmation(order);
        }

        public void ShowOrders()
        {
            if (_orderService.IsEmpty)
            {
                Console.WriteLine("\nNo hay órdenes registradas.");
                return;
            }

            Console.WriteLine("\n=== ÓRDENES ===");
            foreach (var o in _orderService.Orders)
            {
                Console.WriteLine($"  Orden #{o.Id} - {o.Status} - ${o.Total:F2} - {o.CustomerName} - {o.CreatedAt:g}");
            }
        }

        public void CancelOrder(int orderId)
        {
            var order = _orderService.FindById(orderId);
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
            Console.WriteLine($"OK: Orden #{orderId} cancelada.");
            NotificationService.SendOrderCancellation(order);
        }
    }
}