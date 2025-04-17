using Application.Infrastructure;
using Application.Domain.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Application.Services;
using System.Text.Json.Serialization;
using Stripe;
using Application.Features.Payment;

namespace Application;

public static class DependencyInjection
{
    public static void AddApplication(this WebApplicationBuilder builder)
    {
        // Database
        var DataBaseConnectionString = builder.Configuration.GetConnectionString("Database");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(DataBaseConnectionString);
        });

        // Redis
        builder.Services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
        {
            var RedisConnectionString = builder.Configuration.GetConnectionString("Redis");
            return ConnectionMultiplexer.Connect(RedisConnectionString!);
        });

        builder.Services.AddScoped<ICartRepository, RedisCartRepository>();
        builder.Services.AddScoped<CartService>();

        //  Orders 
        builder.Services.AddScoped<OrderService>();


        // Enum to String 
        builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.Configure<StripeOptions>(builder.Configuration.GetRequiredSection("Stripe"));
        StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
    }

}
