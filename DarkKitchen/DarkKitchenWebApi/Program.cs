using Almacenamiento;
using AlmacenamientoAbstracciones;
using DarkKitchen.Filters;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
    options.Filters.Add<AuthenticationFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ISignUpUserService, SignUpService>();
builder.Services.AddScoped<IModifyUserService, ModifyUserService>();
builder.Services.AddScoped<IDeleteUserService, DeleteUserService>();
builder.Services.AddScoped<IFilterUserService, FilterUserService>();
builder.Services.AddScoped<IAdminValidationService, AdminValidationService>();
builder.Services.AddScoped<ISessionsValidationService, SessionsValidationService>();
builder.Services.AddSingleton<IInitializationService, InitializationService>();

// Filters
builder.Services.AddScoped<AuthenticationFilter>();

// Helpers / validators
builder.Services.AddScoped<IPhoneValidator, PhoneValidatorUruguay>();
builder.Services.AddScoped<IReloj, WatchSystem>();

// Repositories
builder.Services.AddSingleton<IRepositoryUser, RepositoryUser>();
builder.Services.AddSingleton<IRepositorySession, RepositorySession>();

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
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
