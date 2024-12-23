﻿namespace Basket.API.Data
{
    public class BasketRepository(IDocumentSession session) : IBasketRepository
    {

        public async Task<bool> DeleteBasket(string username, CancellationToken cancellationToken)
        {
            session.Delete<ShoppingCart>(username);
            await session.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<ShoppingCart> GetBasket(string username, CancellationToken cancellationToken)
        {
            var basket = await session.LoadAsync<ShoppingCart>(username, cancellationToken);

            return basket is null ? throw new BasketNotFoundException(username) : basket;
        }

        public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken)
        {
            session.Store(basket);
            await session.SaveChangesAsync(cancellationToken);
            return basket;
        }
    }
}
