# MiniEcommerce — Aprendizaje de Diseño de Software en .NET

Aplicación de consola que simula un e-commerce básico. El objetivo no es el producto, sino el proceso: aprender SOLID y patrones de diseño GoF refactorizando progresivamente desde una versión naive.

## Stack

- .NET 10 Console App
- xUnit para tests
- Sin frameworks de DI (manual)

## Estructura

```
src/MiniEcommerce/
├── Program.cs              # Composition Root + CLI loop
├── Product.cs              # Modelo
├── CartItem.cs             # Modelo
├── Order.cs                # Modelo
├── StoreService.cs         # Coordinador (recibe dependencias por constructor)
├── ShoppingCart.cs          # Gestión del carrito
├── ProductCatalog.cs        # Almacén y búsqueda de productos
├── OrderService.cs          # Creación y cancelación de órdenes
├── IDiscountStrategy.cs     # Interfaz para estrategias de descuento
├── Discounts.cs             # Implementaciones: PercentageDiscount, FlatDiscount
├── DiscountCalculator.cs    # Registro y ejecución de estrategias de descuento
├── TaxCalculator.cs         # Cálculo de impuestos (IVA)
├── IOrderNotifier.cs        # Interfaz segregada para notificaciones
└── NotificationService.cs   # Implementaciones: EmailNotifier, LogNotifier

tests/MiniEcommerce.Tests/
├── CalculatorTests.cs       # Tests de descuentos e impuestos
├── ShoppingCartTests.cs     # Tests del carrito
├── OrderServiceTests.cs     # Tests de órdenes
└── ProductCatalogTests.cs   # Tests del catálogo
```

## Progreso

### SOLID

#### SRP (Single Responsibility Principle)

Se partió de una God Class (`StoreService`) con 7 responsabilidades y se refactorizó progresivamente:

| Commit                                         | Qué se hizo                                   |
| ---------------------------------------------- | --------------------------------------------- |
| `feat: versión naive`                          | Todo en Program.cs — una sola clase hace todo |
| `refactor: DiscountCalculator y TaxCalculator` | Extraer cálculos de negocio                   |
| `refactor: NotificationService`                | Extraer envío de notificaciones               |
| `refactor: ShoppingCart`                       | Extraer gestión del carrito                   |
| `refactor: ProductCatalog`                     | Extraer catálogo de productos                 |
| `refactor: OrderService`                       | Extraer creación/cancelación de órdenes       |
| `refactor: modelos a archivos propios`         | Separar clases de Program.cs                  |

**Resultado:** `StoreService` pasó de God Class a coordinador — solo conecta servicios y maneja la salida a consola.

#### OCP (Open/Closed Principle) + Strategy Pattern

| Commit                                   | Qué se hizo                                                                                                              |
| ---------------------------------------- | ------------------------------------------------------------------------------------------------------------------------ |
| `refactor: OCP + Strategy en descuentos` | Descuentos extensibles sin modificar código existente. Interfaz `IDiscountStrategy` con implementaciones intercambiables |

**Resultado:** Agregar un nuevo tipo de descuento = crear una clase nueva y registrarla. No se toca código existente.

#### LSP (Liskov Substitution Principle)

Concepto cubierto: prefiere interfaces sobre herencia. Las implementaciones de `IDiscountStrategy` y `IOrderNotifier` son sustituibles sin romper el comportamiento esperado.

#### ISP (Interface Segregation Principle)

| Commit                            | Qué se hizo                                                                                                                      |
| --------------------------------- | -------------------------------------------------------------------------------------------------------------------------------- |
| `refactor: ISP en notificaciones` | Interfaz `IOrderNotifier` cohesiva en vez de una interfaz gorda. Implementaciones independientes: `EmailNotifier`, `LogNotifier` |

**Resultado:** Cada notifier hace una sola cosa. Agregar un canal nuevo (SMS, Push) = nueva clase que implementa `IOrderNotifier`.

#### DIP (Dependency Inversion Principle)

| Commit                                             | Qué se hizo                                                                                                    |
| -------------------------------------------------- | -------------------------------------------------------------------------------------------------------------- |
| `refactor: DIP - inyección de dependencias manual` | `StoreService` y `OrderService` reciben dependencias por constructor. `Program.cs` actúa como Composition Root |

**Resultado:** Las clases de alto nivel dependen de abstracciones, no de implementaciones concretas. Las dependencias se ensamblan en un solo lugar (`Program.cs`).
