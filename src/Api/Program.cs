using Application;
using Application.Features.Cart;
using Application.Features.Catalog;
using Application.Features.Orders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//TODO Move to DependencyInjection
// Habilitar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalHost", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.AddApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseCors("AllowLocalHost");

app.UseHttpsRedirection();

app.AddCatalogEndpoints();
app.AddCartEndpoints();
app.AddOrderEndpoints();

app.Run();

