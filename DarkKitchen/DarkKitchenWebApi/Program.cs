using Almacenamiento;
using DarkKitchen.Filters;
using FactoryService;
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
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();

// Services
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ISignUpUserService, SignUpService>();
builder.Services.AddScoped<IModifyUserService, ModifyUserService>();
builder.Services.AddScoped<IDeleteUserService, DeleteUserService>();
builder.Services.AddScoped<IFilterUserService, FilterUserService>();
builder.Services.AddScoped<IAdminValidationService, AdminValidationService>();
builder.Services.AddScoped<ISessionValidationService, SessionValidationService>();
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
