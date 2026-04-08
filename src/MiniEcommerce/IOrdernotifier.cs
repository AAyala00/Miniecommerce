namespace MiniEcommerce;

public interface IOrdernotifier
{
    void OnOrderCreated(Order order);
    void OnOrderCancelled(Order order);
}