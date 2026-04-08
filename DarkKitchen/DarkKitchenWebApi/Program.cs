using Almacenamiento;
using AlmacenamientoAbstracciones;
using DarkKitchen.Filters;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ISignUpUserService, SignUpService>();
builder.Services.AddScoped<IModifyUserService, ModifyUserService>();
builder.Services.AddScoped<IDeleteUserService, DeleteUserService>();
builder.Services.AddScoped<IFilterUserService, FilterUserService>();
builder.Services.AddScoped<IAdminValidationService, AdminValidationService>();

// Helpers / validators
builder.Services.AddScoped<IPhoneValidator, PhoneValidatorUruguay>();
builder.Services.AddScoped<IReloj, WatchSystem>();

// Repositories
builder.Services.AddSingleton<IRepositoryUser, RepositoryUser>();
builder.Services.AddSingleton<IRepositorySession, RepositorySession>();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
