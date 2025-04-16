namespace Application.Domain.Interfaces;

using Application.Domain.Entities.Cart;

public interface ICartRepository
{
    Task<Cart?> GetCartAsync(string userId);
    Task<Cart> SetCartAsync(Cart cart);
    Task<bool> DeleteCartAsync(string userId);
}
