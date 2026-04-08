# MiniEcommerce — Aprendizaje de Diseño de Software en .NET

Aplicación de consola que simula un e-commerce básico. El objetivo no es el producto, sino el proceso: aprender SOLID y patrones de diseño GoF refactorizando progresivamente desde una versión naive.

## Stack

- .NET 10 Console App
- xUnit para tests
- Sin frameworks de DI (manual)

## Estructura

```
src/MiniEcommerce/
├── Program.cs              # CLI loop (entrada/salida)
├── Product.cs              # Modelo
├── CartItem.cs             # Modelo
├── Order.cs                # Modelo
├── StoreService.cs         # Coordinador (delega a los servicios)
├── ShoppingCart.cs          # Gestión del carrito
├── ProductCatalog.cs        # Almacén y búsqueda de productos
├── OrderService.cs          # Creación y cancelación de órdenes
├── DiscountCalculator.cs    # Cálculo de descuentos
├── TaxCalculator.cs         # Cálculo de impuestos (IVA)
└── NotificationService.cs   # Envío de notificaciones

tests/MiniEcommerce.Tests/
├── CalculatorTests.cs       # Tests de descuentos e impuestos
├── ShoppingCartTests.cs     # Tests del carrito
├── OrderServiceTests.cs     # Tests de órdenes
└── ProductCatalogTests.cs   # Tests del catálogo
```

## Progreso

### SRP (Single Responsibility Principle)

Se partió de una God Class (`StoreService`) con 7 responsabilidades y se refactorizó progresivamente:

| Commit                                         | Qué se hizo                                           |
| ---------------------------------------------- | ----------------------------------------------------- |
| `feat: versión naive`                          | Todo en Program.cs — una sola clase hace todo         |
| `refactor: DiscountCalculator y TaxCalculator` | Extraer cálculos de negocio                           |
| `refactor: NotificationService`                | Extraer envío de notificaciones                       |
| `refactor: ShoppingCart`                       | Extraer gestión del carrito                           |
| `refactor: ProductCatalog`                     | Extraer catálogo de productos                         |
| `refactor: OrderService`                       | Extraer creación/cancelación de órdenes               |
| `refactor: modelos a archivos propios`         | Separar clases de Program.cs                          |
| `refactor: OCP + Strategy en descuentos`       | Descuentos extensibles sin modificar código existente |

**Resultado:** `StoreService` pasó de God Class a coordinador — solo conecta servicios y maneja la salida a consola. Cada clase tiene una sola razón para cambiar.
