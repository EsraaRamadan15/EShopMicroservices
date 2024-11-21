namespace Basket.API.Data
{
    public interface IBasketRepository
    {
        Task<ShoppingCart> GetBasket(string username, CancellationToken cancellationToken);
        Task<bool> DeleteBasket(string username, CancellationToken cancellationToken);
        Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken);
    }
}
