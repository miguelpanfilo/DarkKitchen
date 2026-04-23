using DarkKitchen.Filters;
using DarkKitchenWebApi.Extensions;
using FactoryService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
    options.Filters.Add<AuthenticationFilter>();
});

builder.Services.AddCustomSwagger();
builder.Services.AddDarkKitchenDependencies(builder.Configuration);
builder.Services.AddScoped<AuthenticationFilter>();

var app = builder.Build();

app.InitializeDatabase();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
