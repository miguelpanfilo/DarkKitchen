namespace Domain;

public class User
{
    public int Id { get; set; }
    public string Name { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public Email Email { get; private set; }
    public Phone Phone { get; private set; }
    public Password Password { get; private set; }
    public UserRoles Role { get; private set; }
    public UserStatus Status { get; private set; }

    private User()
    {
        Email = null!;
        Phone = null!;
        Password = null!;
    }

    private const int MinLastNameLength = 3;
    private const int MaxLastNameLength = 25;

    private static readonly string LastNameEmptyMessage = "El apellido no puede estar vacío";

    private static readonly string LastNameLengthMessage =
        $"El apellido debe tener entre {MinLastNameLength} y {MaxLastNameLength} caracteres";

    public static Result<User> MakeClient(string name, string lastName,
        string email, Phone phone, string password) => BuildUser(name, lastName, email, phone, password, UserRoles.Client);

    public static Result<User> MakeIntern(string name, string lastName,
        string email, Phone phone, string password, UserRoles role)
    {
        if(role == UserRoles.Client)
        {
            return Result<User>.Fail("El rol no es válido para usuario interno");
        }

        return BuildUser(name, lastName, email, phone, password, role);
    }

    private static Result<User> BuildUser(string name, string lastName, string email, Phone phone, string password,
        UserRoles role)
    {
        var nameResult = ValidateName(name);
        if(!nameResult.Success)
        {
            return
                Result<User>.Fail(nameResult.ErrorMessage!);
        }

        var lastNameResult = ValidateLastName(lastName);
        if(!lastNameResult.Success)
        {
            return
                Result<User>.Fail(lastNameResult.ErrorMessage!);
        }

        var emailResult = Email.TryMakeEmail(email);
        if(!emailResult.Success)
        {
            return
                Result<User>.Fail(emailResult.ErrorMessage!);
        }

        var passwordResult = Password.TryMakePassword(password);
        if(!passwordResult.Success)
        {
            return
                Result<User>.Fail(passwordResult.ErrorMessage!);
        }

        return Result<User>.Ok(new User(name, lastName,
            emailResult.Value!, phone, passwordResult.Value!, role));
    }

    private User(string name, string lastName, Email email, Phone phone, Password password, UserRoles role)
    {
        Name = name.Trim();
        LastName = lastName.Trim();
        Email = email;
        Phone = phone;
        Password = password;
        Role = role;
        Status = UserStatus.Active;
    }

    public void Deactivate()
    {
        Status = UserStatus.Inactive;
    }

    private static Result<bool> ValidateName(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            return Result<bool>.Fail("Nombre no puede estar vacío");
        }

        if(name.Any(char.IsDigit))
        {
            return Result<bool>.Fail("Nombre no puede contener números");
        }

        return Result<bool>.Ok(true);
    }

    private static Result<bool> ValidateLastName(string lastName)
    {
        if(string.IsNullOrWhiteSpace(lastName))
        {
            return Result<bool>.Fail(LastNameEmptyMessage);
        }

        var trimmedLastName = lastName.Trim();

        if(trimmedLastName.Length < MinLastNameLength || trimmedLastName.Length > MaxLastNameLength)
        {
            return Result<bool>.Fail(LastNameLengthMessage);
        }

        return Result<bool>.Ok(true);
    }

    public Result<bool> ModifyData(string name, string lastName, Phone phone, string email, UserRoles role)
    {
        var nameResult = ValidateName(name);
        if(!nameResult.Success)
        {
            return Result<bool>.Fail(nameResult.ErrorMessage!);
        }

        var lastNameResult = ValidateLastName(lastName);
        if(!lastNameResult.Success)
        {
            return Result<bool>.Fail(lastNameResult.ErrorMessage!);
        }

        var emailResult = Email.TryMakeEmail(email);
        if(!emailResult.Success)
        {
            return Result<bool>.Fail(emailResult.ErrorMessage!);
        }

        Name = name.Trim();
        LastName = lastName.Trim();
        Phone = phone;
        Email = emailResult.Value!;
        Role = role;

        return Result<bool>.Ok(true);
    }

    public bool VerifyPassword(string password)
    {
        return Password.VerifyHash(password);
    }

    public void ModifyPassword(Password newPassword)
    {
        Password = newPassword;
    }
}
