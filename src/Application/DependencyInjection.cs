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
using Microsoft.AspNetCore.Hosting;

namespace Application;

public static class DependencyInjection
{
    public static void AddApplication(this WebApplicationBuilder builder)
    {
        // Kestrel
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Configure(builder.Configuration.GetSection("Kestrel"));
        });

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

        var stripeOptions = new StripeOptions
        {
            ApiKey = Environment.GetEnvironmentVariable("Stripe__ApiKey")!,
            WebhookSecret = Environment.GetEnvironmentVariable("Stripe__WebhookSecret")!
        };

        builder.Services.Configure<StripeOptions>(options =>
        {
            options.ApiKey = stripeOptions.ApiKey;
            options.WebhookSecret = stripeOptions.WebhookSecret;
        });

        StripeConfiguration.ApiKey = stripeOptions.ApiKey;

        // Payment
        builder.Services.AddScoped<IPayment, PaymentService>();
    }

}
