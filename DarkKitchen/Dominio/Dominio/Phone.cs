using Dominio.Exceptions;

namespace Dominio;

public sealed class Phone
{
    public string Value { get; }

    private Phone(string value)
    {
        Value = value;
    }

    public static Phone MakePhone(string number)
    {
        if(string.IsNullOrWhiteSpace(number))
        {
            throw new UserValidationException("Phone cannot be empty");
        }

        var phoneNormalized = number.Trim();

        if(!phoneNormalized.All(char.IsDigit))
        {
            throw new UserValidationException("Phone must contain only digits");
        }

        return new Phone(phoneNormalized);
    }

    public override string ToString()
    {
        return Value;
    }
}
