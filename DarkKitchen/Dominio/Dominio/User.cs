using Dominio.Exceptions;

namespace Dominio;

public class User
{
    public int Id { get; set; }
    public string Name { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public Email Email { get; private set; }
    public Phone Phone { get; private set; }
    public Password Password { get; private set; }
    public UserRoles Role { get; private set; }

    public static User MakeClient(string nombre, string apellido, string email, Phone phone, string clave)
    {
        var emailValido = Email.MakeEmail(email);
        var claveHasheada = Password.MakePassword(clave);

        return new User(nombre, apellido, emailValido, phone, claveHasheada, UserRoles.Client);
    }

    private User(string name, string lastname, Email email, Phone phone, Password password, UserRoles role)
    {
        ValidatorName(name);
        ValidatorLastName(lastname);

        Name = name.Trim();
        LastName = lastname.Trim();
        Email = email;
        Phone = phone;
        Password = password;
        Role = role;
    }

    private static void ValidatorName(string nombre)
    {
        if(string.IsNullOrWhiteSpace(nombre))
        {
            throw new UserException("Name cannot be empty");
        }

        if(nombre.Any(char.IsDigit))
        {
            throw new UserException("Name cannot contain numbers");
        }
    }

    private static void ValidatorLastName(string apellido)
    {
        if(string.IsNullOrWhiteSpace(apellido))
        {
            throw new UserException("Last name cannot be empty");
        }

        if(apellido.Trim().Length < 3 || apellido.Trim().Length > 25)
        {
            throw new UserException("Last name must be between 3 and 25 characters");
        }
    }

    public static User MakeIntern(
        string name,
        string lastname,
        string email,
        Phone phone,
        string password,
        UserRoles role)
    {
        var itsValidRole = role == UserRoles.Preparer || role == UserRoles.Administrative;

        if(!itsValidRole)
        {
            throw new UserException("Internal role is not valid");
        }

        var emailValid = Email.MakeEmail(email);
        var passwordValid = Password.MakePassword(password);

        return new User(name, lastname, emailValid, phone, passwordValid, role);
    }

    public void ModifyName(string nombre)
    {
        ValidatorName(nombre);
        Name = nombre.Trim();
    }

    public void ModifyData(string name, string lastname, Phone phone, string email, UserRoles role)
    {
        ValidatorName(name);
        ValidatorLastName(lastname);

        Name = name.Trim();
        LastName = lastname.Trim();
        Phone = phone;
        Email = Email.MakeEmail(email);
        Role = role;
    }

    public bool VerifyPassword(string clave)
    {
        return Password.VerifyHash(clave);
    }

    public void ModifyPassword(Password nuevaPassword)
    {
        Password = nuevaPassword;
    }
}
