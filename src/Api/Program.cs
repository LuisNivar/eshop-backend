using Application;
using Application.Features.Cart;
using Application.Features.Catalog;
using Application.Features.Orders;
using Application.Features.Payment;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

DotNetEnv.Env.Load("./.env.local");

builder.AddApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//TODO: Fluent validations
app.AddCatalogEndpoints();
app.AddCartEndpoints();
app.AddOrderEndpoints();
app.AddPaymentEndpoint();

app.Run();

