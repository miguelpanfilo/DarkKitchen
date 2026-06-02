using System.Text.Json.Serialization;
using DarkKitchen;
using DarkKitchen.Filters;
using FactoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ExceptionFilter>();
        options.Filters.Add<AuthenticationFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        return new BadRequestObjectResult(new
        {
            executionSuccessful = false,
            message = "El cuerpo de la solicitud es inválido o está vacío."
        });
    };
});

builder.Services.AddCustomSwagger();
builder.Services.AddDarkKitchenDependencies(builder.Configuration);
builder.Services.AddSignalRServices();
builder.Services.AddWebApiServices();
builder.Services.AddCorsPolicy();

builder.Services.AddScoped<ExceptionFilter>();
builder.Services.AddScoped<AuthenticationFilter>();

var app = builder.Build();

app.StartProductImportersWatcher();
app.InitializeDatabase();
app.UseCorsPolicy();
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var photosPath = builder.Configuration["PhotoStorage:Path"]!;
Directory.CreateDirectory(photosPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(photosPath),
    RequestPath = "/photos"
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapApplicationEndpoints();

app.Run();
