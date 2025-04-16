namespace Application.Services;

using Application.Domain.Interfaces;
using CartEntity = Domain.Entities.Cart.Cart;
using CartItemEntity = Domain.Entities.Cart.CartItem;

public class CartService
{
    private readonly ICartRepository _repository;
    private readonly ApplicationDbContext _db;

    public CartService(ICartRepository repository, ApplicationDbContext db)
    {
        _repository = repository;
        _db = db;
    }

    public Task<CartEntity?> GetAsync(string userId) => _repository.GetCartAsync(userId);

    public async Task<CartEntity> CreateAsync(CartEntity cart)
    {
        var items = cart.Items;

        foreach (var item in items)
        {
            var product = await _db.Products.FindAsync(item.ProductId)
             ?? throw new Exception("Product not found!");
        }
        return await _repository.SetCartAsync(cart);
    }

    //public Task<CartEntity> UpdateAsync(CartEntity cart) => _repository.SetCartAsync(cart);
    public Task<bool> DeleteAsync(string userId) => _repository.DeleteCartAsync(userId);

    public async Task<CartEntity> UpdateQuantityItemAsync(string userId, int productId, int quantity)
    {
        var cart = await _repository.GetCartAsync(userId) ?? throw new Exception("Cart not found!");
        var existing = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (existing is not null)
            existing.Quantity += quantity;
        else throw new Exception("Product not found!");

        return await _repository.SetCartAsync(cart);
    }

    public async Task<CartEntity> AddItemAsync(string userId, int productId, int quantity)
    {
        var cart = await _repository.GetCartAsync(userId) ?? new CartEntity { UserId = userId };

        var product = await _db.Products.FindAsync(productId)
            ?? throw new Exception("Product not found!");

        var existing = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
            existing.Quantity += quantity;
        else
            cart.Items.Add(new CartItemEntity
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                ImageUrl = product.ImageUrl,
                Quantity = quantity
            });

        return await _repository.SetCartAsync(cart);
    }

    public async Task<bool> RemoveItemAsync(string userId, int productId)
    {
        var cart = await _repository.GetCartAsync(userId);
        if (cart is null) return false;

        cart.Items.RemoveAll(i => i.ProductId == productId);
        await _repository.SetCartAsync(cart);
        return true;
    }
}

