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