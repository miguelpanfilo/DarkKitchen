using Dominio.Excepciones;

namespace Dominio;

public class Phone
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
            throw new UserException("Phone cannot be empty");
        }

        var phoneNormalized = number.Trim();

        if(!phoneNormalized.All(char.IsDigit))
        {
            throw new UserException("Phone must contain only digits");
        }

        return new Phone(phoneNormalized);
    }

    public override string ToString()
    {
        return Value;
    }
}
