using Almacenamiento;
using AlmacenamientoAbstracciones;
using DarkKitchen.Filters;
using Dominio;
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
builder.Services.AddScoped<ISessionValidationService, SessionValidationService>();

// Filters
builder.Services.AddScoped<AuthenticationFilter>();

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

// Seed admin en memoria
using(var scope = app.Services.CreateScope())
{
    var userRepository = scope.ServiceProvider.GetRequiredService<IRepositoryUser>();

    if(!userRepository.EmailExist("admin@gmail.com"))
    {
        var phoneResult = Phone.TryMakePhone("099123456");

        if(phoneResult.Success)
        {
            var adminResult = User.MakeIntern(
                "Admin",
                "Principal",
                "admin@gmail.com",
                phoneResult.Value!,
                "PasswordSeguraee3!",
                UserRoles.Administrative);

            if(adminResult.Success)
            {
                userRepository.Add(adminResult.Value!);
            }
        }
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
