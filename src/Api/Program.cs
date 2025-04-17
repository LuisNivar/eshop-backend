using Application;
using Application.Features.Cart;
using Application.Features.Catalog;
using Application.Features.Orders;
using Application.Features.Payment;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.AddApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.AddCatalogEndpoints();
app.AddCartEndpoints();
app.AddOrderEndpoints();
app.AddPaymentEndpoint();

app.Run();

