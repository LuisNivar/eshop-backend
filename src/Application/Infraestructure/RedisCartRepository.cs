using System.Text.Json;
using Application.Domain.Entities.Cart;
using Application.Domain.Interfaces;
using StackExchange.Redis;

namespace Application.Infraestructure;

public class RedisCartRepository : ICartRepository
{
    private readonly IDatabase _db;
    private readonly string _prefix = "cart:";

    public RedisCartRepository(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task<Cart?> GetCartAsync(string userId)
    {
        var data = await _db.StringGetAsync(_prefix + userId);
        if (data.IsNullOrEmpty) return null;
        return JsonSerializer.Deserialize<Cart>(data!);
    }

    public async Task<Cart> SetCartAsync(Cart cart)
    {
        var key = _prefix + cart.UserId;
        var data = JsonSerializer.Serialize(cart);
        await _db.StringSetAsync(key, data);
        return cart;
    }

    public async Task<bool> DeleteCartAsync(string userId)
    {
        return await _db.KeyDeleteAsync(_prefix + userId);
    }
}

