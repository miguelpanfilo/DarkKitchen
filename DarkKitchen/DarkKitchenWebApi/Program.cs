using Almacenamiento;
using AlmacenamientoAbstracciones;
using DarkKitchen.Filters;
using FactoryService;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
    options.Filters.Add<AuthenticationFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Orders
builder.Services.AddScoped<IOrderFacade, OrderFacade>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();

// Products (in-memory implementations)
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductPhotoStorage, ProductPhotoStorage>();
builder.Services.AddScoped<ProductPhotoService>();

builder.Services.AddDarkKitchenDependencies(builder.Configuration);
builder.Services.AddScoped<AuthenticationFilter>();

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SqlContext>();
    context.Database.Migrate();

    var initializationService = scope.ServiceProvider.GetRequiredService<IInitializationService>();
    initializationService.EnsureInitialized();
}

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
